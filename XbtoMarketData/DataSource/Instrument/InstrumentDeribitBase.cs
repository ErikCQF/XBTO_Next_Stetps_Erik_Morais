using System.Text.Json.Serialization;

namespace XbtoMarketData.DataSource.Instrument
{
    /// <summary>
    /// Only basic properties
    /// </summary>
    public class InstrumentDeribitBase
    {
        [JsonPropertyName("id")]
        public virtual int Id { get; set; }

        [JsonPropertyName("kind")]
        public virtual string Kind { get; set; } = string.Empty;

        [JsonPropertyName("instrument_name")]
        public virtual string InstrumentName { get; set; } = string.Empty;

        [JsonPropertyName("base_currency")]
        public virtual string BaseCurrency { get; set; } = string.Empty;

    }
}