namespace XbtoMarketData.DataSource.Price
{
    public interface IPriceDeribitDataSource
    {
        Task<PriceDeribit?> GetLastPrice(string instrumentName);
    }
}
