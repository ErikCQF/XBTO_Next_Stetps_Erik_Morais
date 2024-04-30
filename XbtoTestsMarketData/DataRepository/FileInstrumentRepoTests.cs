using XbtoMarketData.DataRepository.Instrument;

namespace DataRepository.Instrument;
public class FileInstrumentRepoTests
{
    [Fact]
    public async Task AddUpdate_ShouldAddNewInstrument_WhenInstrumentDoesNotExist()
    {
        // Arrange
        var filePath = "test_instruments.json";
        var repo = new FileInstrumentRepo(filePath);
        var instrument = new InstrumentDb
        {
            InstrumentName = "BTC",
            BaseCurrency = "USD",
            Kind = "Crypto"
        };

        // Act
        var result = await repo.AddUpdate(instrument);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("BTC", result.InstrumentName);

        // Clean up
        File.Delete(filePath);
    }

    [Fact]
    public async Task AddUpdate_ShouldUpdateExistingInstrument_WhenInstrumentExists()
    {
        // Arrange
        var filePath = "test_instruments.json";
        var repo = new FileInstrumentRepo(filePath);
        var existingInstrument = new InstrumentDb
        {
            InstrumentName = "BTC",
            BaseCurrency = "USD",
            Kind = "Crypto"
        };
        await repo.AddUpdate(existingInstrument); // Add existing instrument
        var updatedInstrument = new InstrumentDb
        {
            InstrumentName = "BTC",
            BaseCurrency = "EUR",
            Kind = "Crypto"
        };

        // Act
        var result = await repo.AddUpdate(updatedInstrument);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("BTC", result.InstrumentName);
        Assert.Equal("EUR", result.BaseCurrency);

        // Clean up
        File.Delete(filePath);
    }

    [Fact]
    public async Task GetToMonitor_ShouldReturnListOfInstruments()
    {
        // Arrange
        var filePath = "test_instruments.json";
        var repo = new FileInstrumentRepo(filePath);
        var instrument1 = new InstrumentDb
        {
            InstrumentName = "BTC",
            BaseCurrency = "USD",
            Kind = "Crypto"
        };
        var instrument2 = new InstrumentDb
        {
            InstrumentName = "ETH",
            BaseCurrency = "USD",
            Kind = "Crypto"
        };
        await repo.AddUpdate(instrument1);
        await repo.AddUpdate(instrument2);

        // Act
        var result = await repo.GetToMonitor();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Clean up
        File.Delete(filePath);
    }
}
