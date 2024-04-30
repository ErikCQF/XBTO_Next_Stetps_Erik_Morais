namespace XbtoMarketData.DataRepository.Instrument
{
    public interface IInstrumentRepo
    {
        Task<InstrumentDb> AddUpdate(InstrumentDb instrument);
        Task<List<InstrumentDb>> GetToMonitor();
    }
}
