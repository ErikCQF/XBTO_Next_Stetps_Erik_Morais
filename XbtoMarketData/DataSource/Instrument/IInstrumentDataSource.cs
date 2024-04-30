namespace XbtoMarketData.DataSource.Instrument
{
    public interface IInstrumentDataSource
    {
        Task<InstrumentDeribitBase?> Get(string instrumentName);
    }
}
