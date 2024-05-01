using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XbtoMarketData.DataRepository.Instrument;
using XbtoMarketData.DataRepository.Price;
using XbtoMarketData.Service.Data.Models;
using XbtoMarketData.Service.Monitor;

namespace XbtoMarketData.Service.Data
{
    public class PriceService : IPriceService
    {
        private readonly IInstrumentRepo _instrumentRepo;
        private readonly IPriceRepo _priceRepo;
        private readonly IPriceMonitor _priceMonitor;

        public PriceService(IInstrumentRepo instrumentRepo, IPriceRepo priceRepo, IPriceMonitor priceMonitor)
        {
            this._instrumentRepo = instrumentRepo;
            this._priceRepo = priceRepo;
            this._priceMonitor = priceMonitor;
        }

        public async Task<GetPriceResponse> LastPrice(GetPriceRequest request)
        {
            if (request?.InstrumentName == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var instrument = await _instrumentRepo.GetByName(request.InstrumentName);
            var price = await _priceRepo.LastPrice(request.InstrumentName);

            if (instrument == null)
            {
                return new GetPriceResponse();
            }

            var response = new GetPriceResponse
            {
                LastPriceModel = new LastPriceModel
                {
                    InstrumentName = instrument?.InstrumentName,
                    BaseCurrency = instrument?.BaseCurrency,
                    Kind = instrument?.Kind,
                    MarkPrice = price.MarkPrice,
                    Price = price.Price
                }
            };

            return await Task.FromResult(response);

        }

        public async Task<MonitorPriceRespose> MonitorPrice(MonitorPriceRequest request)
        {
            this._priceMonitor.MonitorPrice(request.InstrumentName);

            return await Task.FromResult(new MonitorPriceRespose
            {
                Accepted = true,
            });
        }
    }
}
