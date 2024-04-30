namespace XbtoMarketData.DataRepository.Price
{
    public interface IPriceRepo
    {
        Task<PriceDb> AddUpdate(PriceDb instrument);
    }
}
