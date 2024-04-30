using Microsoft.Extensions.Options;
using Moq;
using System.Reactive.Linq;
using XbtoMarketData.DataRepository.Instrument;
using XbtoMarketData.DataRepository.Price;
using XbtoMarketData.DataSource;
using XbtoMarketData.DataSource.Instrument;
using XbtoMarketData.DataSource.Price;
using XbtoMarketData.Service;
using XbtoMarketData.Utils;

namespace XbtoTestsMarketData.Services
{
    public class PriceMonitorIntegratedTest
    {

        private Mock<IInstrumentRepo> _instrumentRepoMock = new Mock<IInstrumentRepo>();
        private Mock<IPriceRepo> _priceRepoMock = new Mock<IPriceRepo>();
        private Mock<IInstrumentDataSource> _instrumentDataSourceMock = new Mock<IInstrumentDataSource>();
        private Mock<IPriceDeribitDataSource> _priceDataSourceMock = new Mock<IPriceDeribitDataSource>();
        private Mock<IDateProvider> _dateProviderMock = new Mock<IDateProvider>();

        string apiKey = "your_api_key";
        string apiSecret = "your_api_secret";
        string wsUrl = "wss://test.deribit.com/ws/api/v2";

        private readonly Mock<IOptions<DeribitOSettings>> deribitOSettingsMock = new Mock<IOptions<DeribitOSettings>>();


        [Fact]
        public async Task TestMonitor_No_Rate_Limite_Integrated()
        {
            //Arrage
            var instrumentName = "BTC-PERPETUAL";
            var instrumentDB = new InstrumentDb()
            {
                InstrumentName = instrumentName
            };

            var instrumentDBList = new List<InstrumentDb>()
            {
                instrumentDB
            };

            _instrumentRepoMock
                .Setup(a => a.GetToMonitor()).ReturnsAsync(instrumentDBList);

            _instrumentRepoMock
                .Setup(a => a.AddUpdate(It.IsAny<InstrumentDb>()))
                .ReturnsAsync(instrumentDB);

            _priceRepoMock
                .Setup(a => a.AddUpdate(It.IsAny<PriceDb>()))
                .ReturnsAsync(new PriceDb());


            _priceRepoMock
                .Setup(a => a.AddUpdate(It.IsAny<PriceDb>()));

            var dateProvider = new DateProvider();

            var settings = new DeribitOSettings
            {
                ApiKey = apiKey,
                ApiSecret = apiSecret,
                WsUrl = wsUrl,
            };

            deribitOSettingsMock
                .Setup(a => a.Value)
                .Returns(settings);

            var priceDataSource = new PriceDeribitDataSource(deribitOSettingsMock.Object);

            var priceMonitor = new PriceMonitor(
                _instrumentRepoMock.Object,
                _priceRepoMock.Object,
                _instrumentDataSourceMock.Object,
                priceDataSource,
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


            await Task.Delay(10000);

            //Assert
            Assert.True(eventRaised);

        }



        [Fact]
        public async Task TestMonitor_No_Rate_Limite_Integrated_Using_MonitorADD_Fuctionn()
        {
            //Arrage
            var instrumentName_A = "BTC-PERPETUAL";
            var instrumentName_B = "BTC_USDC";


            var unique = Guid.NewGuid().ToString();


            var instrumentRepo = new FileInstrumentRepo($"instrumentTEST{unique}.json");

            var priceRepo = new FilePriceRepo($"priceTEST{unique}.json");




            var dateProvider = new DateProvider();

            var settings = new DeribitOSettings
            {
                ApiKey = apiKey,
                ApiSecret = apiSecret,
                WsUrl = wsUrl,
            };

            deribitOSettingsMock
                .Setup(a => a.Value)
                .Returns(settings);

            var priceDataSource = new PriceDeribitDataSource(deribitOSettingsMock.Object);
            var instrumentDataSource = new InstrumentDeribitDataSource(deribitOSettingsMock.Object);

            var priceMonitor = new PriceMonitor(
                instrumentRepo,
                priceRepo,
                instrumentDataSource,
                priceDataSource,
                dateProvider);


            PriceDeribit receivedPrice = new PriceDeribit();
            bool eventRaised = false;

            List<PriceDeribit> prices = new List<PriceDeribit>();

            priceMonitor.PriceChanged += (price) =>
            {
                prices.Add(price);
                receivedPrice = price;
                eventRaised = true;
            };

            //Act
            var cancToken = new CancellationTokenSource();

            var running = priceMonitor.Start(1, 1000000, cancToken.Token);


            await Task.Delay(1000);

            priceMonitor.MonitorPrice(instrumentName_A);

            await Task.Delay(1000);

            priceMonitor.MonitorPrice(instrumentName_B);

            await Task.Delay(10000);

            //Assert
            Assert.True(eventRaised);
            Assert.True(prices.Count > 4);


        }

    }
}