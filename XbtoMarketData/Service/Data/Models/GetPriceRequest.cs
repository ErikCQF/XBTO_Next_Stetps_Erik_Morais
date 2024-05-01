using System.Text.Json.Serialization;

namespace XbtoMarketData.Service.Data.Models
{
    public class GetPriceRequest
    {
        public string InstrumentName { get; set; } = string.Empty; 
    }
}