namespace XbtoMarketData.Service.Data.Models
{
    public class LastPriceModel
    {        
        public double? Price { get; set; }        
        public double? MarkPrice { get; set; }        
        public string? InstrumentName { get; set; } = string.Empty;
        public string? Kind { get; set; } = string.Empty;
        public string? BaseCurrency { get; set; } = string.Empty;
    }
}