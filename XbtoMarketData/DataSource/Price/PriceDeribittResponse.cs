using System.Text.Json.Serialization;

namespace XbtoMarketData.DataSource.Price
{
    public class PriceDeribittResponse
    {
        [JsonPropertyName("jsonrpc")]
        public string? JsonRpc { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("result")]
        public PriceDeribitResult? Result { get; set; }
    }

}




