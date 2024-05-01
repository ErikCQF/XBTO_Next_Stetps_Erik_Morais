namespace XbtoMarketData.DataRepository.Instrument
{
    public interface IInstrumentRepo
    {
        Task<InstrumentDb> AddUpdate(InstrumentDb instrument);
        Task<List<InstrumentDb>?> GetToMonitor();
        Task<InstrumentDb?> GetByName(string instrumentName);
    }
}
