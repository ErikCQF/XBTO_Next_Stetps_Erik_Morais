using Microsoft.Extensions.Options;
using Moq;
using XbtoMarketData.DataSource;
using XbtoMarketData.DataSource.Instrument;

namespace DataSource.Instrument
{
    public class InstrumentDeribitDataSourceTest
    {
        string apiKey = "your_api_key";
        string apiSecret = "your_api_secret";
        string wsUrl = "wss://test.deribit.com/ws/api/v2";

        private readonly Mock<IOptions<DeribitOSettings>> deribitOSettingsMock = new Mock<IOptions<DeribitOSettings>>();

        [Theory]
        [InlineData("BTC-13JAN23-16000-P")]

        public async Task Get_Instruments_Valid_Names(string instrumentName)
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


            var dataSource = new InstrumentDeribitDataSource(deribitOSettingsMock.Object);

            //Act
            var result = await dataSource.Get(instrumentName);


            //Assert
            Assert.NotNull(result);

            Assert.True(result?.InstrumentName == instrumentName);

        }

        [Theory]
        [InlineData("Invalid Name")]

        public async Task Get_Instruments_NOT_FOUND(string instrumentName)
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


            var dataSource = new InstrumentDeribitDataSource(deribitOSettingsMock.Object);

            //Act
            var result = await dataSource.Get(instrumentName);


            //Assert
            Assert.Null(result);

        }
    }

}
