using System.Text.Json.Serialization;

namespace XbtoMarketData.DataSource.Instrument
{
    public class InstrumentDeribitResponse
    {
        [JsonPropertyName("jsonrpc")]
        public string? JsonRpc { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("result")]
        public InstrumentDeribitBase? Result { get; set; }
    }
}