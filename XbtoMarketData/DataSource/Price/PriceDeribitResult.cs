using System.Text.Json.Serialization;

namespace XbtoMarketData.DataSource.Price
{
    public class PriceDeribitResult
    {
        [JsonPropertyName("trades")]

        public List<PriceDeribit> Trades { get; set; }
        public bool has_more { get; set; }
    }
}