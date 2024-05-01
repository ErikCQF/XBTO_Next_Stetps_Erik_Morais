using XbtoMarketData.DataRepository.Instrument;
using XbtoMarketData.DataRepository.Price;
using XbtoMarketData.DataSource.Instrument;
using XbtoMarketData.DataSource.Price;
using XbtoMarketData.Utils;

namespace XbtoMarketData.Service.Monitor
{
    public class PriceMonitor : IPriceMonitor
    {
        private readonly IInstrumentRepo _instrumentRepo;
        private readonly IPriceRepo _priceRepo;
        private readonly IInstrumentDataSource _instrumentDataSource;
        private readonly IPriceDeribitDataSource _priceDataSource;
        private readonly IDateProvider _dateProvider;
        private int _fetchIntervalSeconds = 0;
        private int _rateLimitPerSecond = 0;

        private object _lock = new object();
        private Dictionary<string, InstrumentDeribitBase> _monitored = new Dictionary<string, InstrumentDeribitBase>();
        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        public PriceMonitor(IInstrumentRepo instrumentRepo,
                             IPriceRepo priceRepo,
                             IInstrumentDataSource instrumentDataSource,
                             IPriceDeribitDataSource PriceDataSource,
                             IDateProvider dateProvider
                             )
        {
            _instrumentRepo = instrumentRepo;
            _priceRepo = priceRepo;
            _instrumentDataSource = instrumentDataSource;
            _priceDataSource = PriceDataSource;
            _dateProvider = dateProvider;
        }

        public event Action<PriceDeribit>? PriceChanged;

        public async void MonitorPrice(string instrumentName)
        {
            //if it is has been monitored, do nothing
            if (_monitored.TryGetValue(instrumentName, out InstrumentDeribitBase? instrument))
            {
                return;
            }

            //get instrument from data source, save it to data store then start to monitor
            var newInstrument = await _instrumentDataSource.Get(instrumentName);

            ///Mappers
            if (newInstrument != null)
            {
                var db = new InstrumentDb
                {
                    BaseCurrency = newInstrument.BaseCurrency ?? string.Empty,
                    InstrumentName = newInstrument.InstrumentName ?? string.Empty,
                    Kind = newInstrument.Kind ?? string.Empty,
                };
                await _instrumentRepo.AddUpdate(db);

                lock (_lock)
                {
                    _monitored.TryAdd(instrumentName, newInstrument);
                }
            }

            // keeping mininmal lock time


        }

        public async Task Start(int fetchIntervalSeconds, int rateLimit, CancellationToken cancellationToken)
        {

            if (fetchIntervalSeconds <= 0 || rateLimit <= 0)
            {
                throw new ArgumentOutOfRangeException("Invalid values for fetchIntervalSeconds or rateLimit. Make sure they are positve numbers ");
            }

            _fetchIntervalSeconds = fetchIntervalSeconds;
            _rateLimitPerSecond = rateLimit;
            _cancellationToken = new CancellationTokenSource();
            var toMonitorList = await _instrumentRepo.GetToMonitor();

            // Loads all Saved Inttruments from DB to be Monitored
            // Thread Safe
            if (toMonitorList!=null && toMonitorList.Any())
            {
                Dictionary<string, InstrumentDeribitBase> monitored = toMonitorList
                    .ToDictionary(a => a.InstrumentName,
                                  a => new InstrumentDeribitBase()
                                  {
                                      InstrumentName = a.InstrumentName,
                                      BaseCurrency = a.BaseCurrency,
                                      Kind = a.Kind
                                  }
                                  );
                lock (_lock)
                {
                    _monitored = monitored;
                }

            }

            //using cancelationg tokens to stop the running process. Another approach should use booleans and volatile 
            while (cancellationToken.IsCancellationRequested == false && _cancellationToken.Token.IsCancellationRequested == false)
            {
                List<InstrumentDeribitBase> instruments = new List<InstrumentDeribitBase>();

                // keeping mininmal lock time. clients could be adding instruments to be monitoring. 
                // Thread Safe
                lock (_lock)
                {
                    instruments = _monitored.Select(a => a.Value).ToList();
                }


                var timeStarted = _dateProvider.Now;
                int totalRequest = 0;
                foreach (var item in instruments)
                {

                    if (cancellationToken.IsCancellationRequested == false && _cancellationToken.Token.IsCancellationRequested)
                    {
                        Console.WriteLine("Cancellation requested. Stopping price fetching.");
                        break;
                    }


                    //Rate Limite. if has reach,it will wait til it can process agains
                    await WaiterRateLimitConstraint(rateLimit, (_dateProvider.Now - timeStarted).TotalSeconds, totalRequest);

            
                    //get the last price
                    var price = await _priceDataSource.GetLastPrice(item.InstrumentName);

                    //TODO : log it for observability
                    if (price == null)
                    {
                      
                        continue;
                    }

                    //TODO Mapper
                    var priceDb = new PriceDb()
                    {
                        InstrumentName = price.InstrumentName,
                        MarkPrice = price.MarkPrice,
                        Price = price.Price,

                    };

                    //Save to Repository
                    //It should be listeing to the event and saving async? Save price is a command
                    await _priceRepo.AddUpdate(priceDb);


                    //Notify the subscribers 
                    PriceChanged?.Invoke(price);

                    totalRequest++;

                }

                // if all have processed before the time for the next trigger, it waits for the time interval
                // otherwise starts right away
                await WaitForNextTrigger(timeStarted);

            }


        }
        /// <summary>
        /// Calculate the remmaining time need to await for next trigger
        /// </summary>
        /// <param name="timeStarted"></param>
        /// <returns></returns>
        protected async Task WaitForNextTrigger(DateTime timeStarted)
        {
            var runningTime = (_dateProvider.Now - timeStarted).TotalSeconds;

            if (runningTime < _fetchIntervalSeconds)
            {
                // Wait before fetching prices again    
                //it just need to wait for the remaiing interval from the last pulse 
                var waitTimeForNextFecth = (int)Math.Ceiling(_fetchIntervalSeconds - runningTime);
                await Task.Delay(waitTimeForNextFecth * 1000);
            }
        }

        /// <summary>
        /// This Function Maximise the using of the rate Limite
        /// it calculates the time it need to await so fix a max limite rate
        /// </summary>
        /// <param name="rateLimit"> Request per Second</param>
        /// <param name="runningTime"> Total time running </param>
        /// <param name="totalRequest">Total Request  </param>
        /// <returns></returns>
        protected virtual async Task WaiterRateLimitConstraint(int rateLimit, double runningTime, int totalRequest)
        {
            if (runningTime <= 0)
            {
                return;
            }
            var usedRate = totalRequest / runningTime;

            while (usedRate >= rateLimit)
            {
                // it is running faster than the allowed rate, so we nneed to slow down
                // To may it concern why the time to wait is calculated like this. 
                // Think rate as velocity, we need to limite the velocity
                // quite trick because it is rates, need to calculate the total time it should wait,
                // so the pipeline will run on max of the rate of rateLimit.

                var ratesDiff = usedRate - rateLimit;
                /*
                  if the limit rate is 80/s, it is running at 100/s
                  the rate diff is 20/s 
                  it means that at current rate, every second, 20 requests are made
                  we need calculate the exact time it need to wait to reduce the rate to the limite rate.
                  The Maths:
                  20 --- 80                      
                  Xs --- 1s 
                  X = 20/80 is the time it need to wait
                */

                //wait time in seconds
                var waitTime = ratesDiff / rateLimit * 1000;

                await Task.Delay(Convert.ToInt32(waitTime));

                //recalculate to make sure it correct
                runningTime = runningTime + waitTime;
                usedRate = totalRequest / runningTime;
            }

            return;
        }

        public void Stop()
        {
            _fetchIntervalSeconds = 0;
            _rateLimitPerSecond = 0;
            _cancellationToken.Cancel();

            lock (_lock)
            {
                _monitored = new Dictionary<string, InstrumentDeribitBase>();
            }


        }
    }
}
