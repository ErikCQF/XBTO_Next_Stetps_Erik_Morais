using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XbtoMarketData.Service.Data.Models;

namespace XbtoMarketData.Service.Data
{
    public interface IPriceService
    {
        Task<GetPriceResponse> LastPrice(GetPriceRequest request);
        Task<MonitorPriceRespose> MonitorPrice(MonitorPriceRequest request);
    }
}
