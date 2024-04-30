using Microsoft.Extensions.Options;
using Moq;
using XbtoMarketData.DataSource;
using XbtoMarketData.DataSource.Price;

namespace DataSource.Price
{
    public class PriceDeribitDataSourceTests
    {
        string apiKey = "your_api_key";
        string apiSecret = "your_api_secret";
        string wsUrl = "wss://test.deribit.com/ws/api/v2";

        private readonly Mock<IOptions<DeribitOSettings>> deribitOSettingsMock = new Mock<IOptions<DeribitOSettings>>();

        [Theory]
        [InlineData("BTC-PERPETUAL")]
        [InlineData("BTC_USDC")]
        
        public async Task Get_LastPrice_Valid_Names(string instrumentName)
        {
            var settings = new DeribitOSettings
            {
                ApiKey = apiKey,
                ApiSecret = apiSecret,
                WsUrl = wsUrl,
            };

            deribitOSettingsMock
                .Setup(a => a.Value)
                .Returns(settings);


            var dataSource = new PriceDeribitDataSource(deribitOSettingsMock.Object);

            //Act
            var result = await dataSource.GetLastPrice(instrumentName);


            //Assert
            Assert.NotNull(result);

            Assert.True(result?.InstrumentName == instrumentName);

            Assert.True(result.Price!=0) ;
            

        }

        [Theory]
        [InlineData("Invalid Name")]

        public async Task Get_LastPrice_NOT_FOUND(string instrumentName)
        {
            var settings = new DeribitOSettings
            {
                ApiKey = apiKey,
                ApiSecret = apiSecret,
                WsUrl = wsUrl,
            };

            deribitOSettingsMock
                .Setup(a => a.Value)
                .Returns(settings);


            var dataSource = new PriceDeribitDataSource(deribitOSettingsMock.Object);

            //Act
            var result = await dataSource.GetLastPrice(instrumentName);


            //Assert
            Assert.Null(result);

        }
    }
}
