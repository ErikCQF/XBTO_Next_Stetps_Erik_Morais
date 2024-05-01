using XbtoMarketData.DataSource.Price;

namespace XbtoMarketData.Service.Monitor
{
    public interface IPriceMonitor
    {
        Task Start(int fetchIntervalSeconds, int rateLimit, CancellationToken cancellationTokens);
        void Stop();

        void MonitorPrice(string instrumentName);
        event Action<PriceDeribit> PriceChanged;
    }
}
