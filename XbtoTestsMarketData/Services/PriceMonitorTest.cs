using Moq;
using System.Reactive.Linq;
using XbtoMarketData.DataRepository.Instrument;
using XbtoMarketData.DataRepository.Price;
using XbtoMarketData.DataSource.Instrument;
using XbtoMarketData.DataSource.Price;
using XbtoMarketData.Service;
using XbtoMarketData.Utils;

namespace XbtoTestsMarketData.Services
{
    public class PriceMonitorTest
    {

        private Mock<IInstrumentRepo> _instrumentRepoMock = new Mock<IInstrumentRepo>();
        private Mock<IPriceRepo> _priceRepoMock = new Mock<IPriceRepo>();
        private Mock<IInstrumentDataSource> _instrumentDataSourceMock = new Mock<IInstrumentDataSource>();
        private Mock<IPriceDeribitDataSource> _priceDataSourceMock = new Mock<IPriceDeribitDataSource>();
        private Mock<IDateProvider> _dateProviderMock = new Mock<IDateProvider>();


        public class PriceMonitorTestRateLimite : PriceMonitor
        {
            public PriceMonitorTestRateLimite(IInstrumentRepo instrumentRepo, IPriceRepo priceRepo, IInstrumentDataSource instrumentDataSource, IPriceDeribitDataSource PriceDataSource, IDateProvider dateProvider) : base(instrumentRepo, priceRepo, instrumentDataSource, PriceDataSource, dateProvider)
            {
            }

            /// <summary>
            /// Returm time waited in seconds
            /// </summary>
            /// <param name="rateLimit"></param>
            /// <param name="runningTime"></param>
            /// <param name="totalRequest"></param>
            /// <returns></returns>
            public async Task<double> RateLimitTest(int rateLimit, double runningTime, int totalRequest)
            {

                var startTime = DateTime.Now;

                await RateLimit(rateLimit, runningTime, totalRequest);

                var endTime = DateTime.Now;

                return (endTime - startTime).TotalSeconds;
            }
        }

        
        [Theory]
        [InlineData(10, 10, 1000, 9)]
        [InlineData(10, 1, 100, 9)]
        [InlineData(11, 10, 10, 0)]
        public async Task Test_Rate_Limite(int rateLimit, double runningTime, int totalRequest, int expectedWaitTimeMs)
        {
            //Arrange

            var dateProvider = new DateProvider();


            var priceMonitor = new PriceMonitorTestRateLimite(
                _instrumentRepoMock.Object,
                _priceRepoMock.Object,
                _instrumentDataSourceMock.Object,
                _priceDataSourceMock.Object,
                dateProvider);


            //Act
            var waitedTime = await priceMonitor.RateLimitTest(rateLimit, runningTime, totalRequest);

            //Assert
            Assert.True(waitedTime >= expectedWaitTimeMs);


        }

        [Fact]
        public async Task TestMonitor_No_Rate_Limite()
        {
            //Arrage
            var instrumentDB = new List<InstrumentDb>()
            {
                new InstrumentDb(){ InstrumentName = "Test 1"},
                new InstrumentDb(){ InstrumentName = "Test 2"}
            };

            _instrumentRepoMock
                .Setup(a => a.GetToMonitor()).ReturnsAsync(instrumentDB);

            _instrumentRepoMock
                .Setup(a => a.AddUpdate(It.IsAny<InstrumentDb>()))
                .ReturnsAsync(new InstrumentDb());

            _priceRepoMock
                .Setup(a => a.AddUpdate(It.IsAny<PriceDb>()))
                .ReturnsAsync(new PriceDb());

            _priceDataSourceMock
                .Setup(a => a.GetLastPrice(It.IsAny<string>()))
                .ReturnsAsync(new PriceDeribit { });


            _priceRepoMock
                .Setup(a => a.AddUpdate(It.IsAny<PriceDb>()));

            var dateProvider = new DateProvider();


            var priceMonitor = new PriceMonitor(
                _instrumentRepoMock.Object,
                _priceRepoMock.Object,
                _instrumentDataSourceMock.Object,
                _priceDataSourceMock.Object,
                dateProvider);


            PriceDeribit receivedPrice = new PriceDeribit();
            bool eventRaised = false;

            priceMonitor.PriceChanged += (price) =>
            {
                receivedPrice = price;
                eventRaised = true;
            };

            //Act
            var cancToken = new CancellationTokenSource();

            var running = priceMonitor.Start(1, 1000000, cancToken.Token);


            await Task.Delay(3000);

            //Assert
            Assert.True(eventRaised);

        }

    }
}