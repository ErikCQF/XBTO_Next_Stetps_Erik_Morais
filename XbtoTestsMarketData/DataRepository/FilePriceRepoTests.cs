
using XbtoMarketData.DataRepository.Price;


namespace XbtoTestsMarketData.DataRepository;

public class FilePriceRepoTests
{
    [Fact]
    public async Task AddUpdate_ShouldAddNewInstrument_WhenInstrumentDoesNotExist()
    {
        // Arrange
        var filePath = "test_prices.json";
        var repo = new FilePriceRepo(filePath);
        var instrument = new PriceDb
        {
            InstrumentName = "BTC",
            Price = 50000,
            MarkPrice = 49500
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
        var filePath = "test_prices.json";
        var repo = new FilePriceRepo(filePath);
        var existingInstrument = new PriceDb
        {
            InstrumentName = "BTC",
            Price = 50000,
            MarkPrice = 49500
        };
        await repo.AddUpdate(existingInstrument); // Add existing instrument
        var updatedInstrument = new PriceDb
        {
            InstrumentName = "BTC",
            Price = 51000,
            MarkPrice = 50500
        };

        // Act
        var result = await repo.AddUpdate(updatedInstrument);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("BTC", result.InstrumentName);
        Assert.Equal(51000, result.Price);

        // Clean up
        File.Delete(filePath);
    }
}
