namespace XbtoMarketData.DataRepository.Price
{
    public class PriceDb
    {
        public double Price { get; set; }

        public double MarkPrice { get; set; }

        public string InstrumentName { get; set; } = string.Empty;
    }
}
