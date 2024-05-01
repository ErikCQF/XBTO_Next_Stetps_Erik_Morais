using Microsoft.AspNetCore.Mvc;
using XbtoMarketData.Service.Data.Models;
using XbtoMarketData.Service.Data;

// Other using directives...

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PriceController : ControllerBase
    {
        private readonly IPriceService _priceService;

        public PriceController(IPriceService priceService)
        {
            _priceService = priceService;
        }

        [HttpGet("lastPrice", Name = "LastPrice")]
        [ProducesResponseType(typeof(void), 204)] // NoContent        
        [ProducesResponseType(typeof(void), 404)] // NotFound
        public async Task<IActionResult> GetLastPrice(string instrumentName)
        {
            var response = await _priceService.LastPrice(new GetPriceRequest
            {
                InstrumentName = instrumentName
            });

            if (response?.LastPriceModel == null)
            {
                return NotFound(); // Return 404 Not Found status
            }

            return Ok(response); // Return 200 OK status with the response

        }

        [HttpPut("MonitorPrice", Name = "MonitorPrice")]
        [ProducesResponseType(typeof(void), 204)] // NoContent        
        [ProducesResponseType(typeof(void), 404)] // NotFound
        public async Task<IActionResult> MonitorPrice(MonitorPriceRequest request)
        {
            var response = await _priceService.MonitorPrice(request);

            if (response.Accepted)
            {
                return Ok(response); // Return 200 OK status with the response

            }
            return NotFound(); // Return 404 Not Found instrumet

        }
    }
}
