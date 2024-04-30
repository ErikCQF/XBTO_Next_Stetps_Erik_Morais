using System.Text.Json.Serialization;

namespace XbtoMarketData.DataSource.Price
{
    public class PriceDeribit
    {
        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("mark_price")]
        public double MarkPrice { get; set; }

        [JsonPropertyName("instrument_name")]
        public string InstrumentName { get; set; } = string.Empty;
    }
}