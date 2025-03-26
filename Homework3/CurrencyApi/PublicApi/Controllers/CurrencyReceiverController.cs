using System.Text.Json;
using Fuse8.BackendInternship.PublicApi.Models;
using Fuse8.BackendInternship.PublicApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Fuse8.BackendInternship.PublicApi.Controllers
{
    /// <summary>
    /// Calls CurrencyAPI
    /// </summary>
    [ApiController]
    [Route("currency/")]
    public class CurrencyReceiverController:ControllerBase
    {
        private readonly IExternalCallerService _caller;
        private readonly DefaultSettings _settings;
        public CurrencyReceiverController(IExternalCallerService caller, IOptionsMonitor<DefaultSettings> settings)
        {
            _caller = caller;
            _settings = settings.CurrentValue;
        }
        /// <summary>
        /// Get exchange rate with default params
        /// </summary>
        /// <response code="200">
        /// if successful
        /// </response>
        /// <response code="404">
        /// if any currency is not found
        /// </response>
        /// <response code="429">
        /// if not enough tokens
        /// </response>
        /// <response code="500">
        /// if unexpected error occurred
        /// </response>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyLoadBase))]
        public async Task<IActionResult> GetBase()
        {
            var stringResponce = await _caller.CallAsync($"latest?currencies={_settings.defaultCurrency}&base_currency={_settings.baseCurrency}");
            ApiResponse castResponse = JsonSerializer.Deserialize<ApiResponse>(stringResponce);
            CurrencyLoadBase body = new(castResponse, _settings.currencyRoundCount);
            return Ok(body);
        }


        /// <summary>
        /// Get exchange rate of specified currency towards base one
        /// </summary>
        /// <param name="currencyCode">specified currency code</param>
        /// <response code="200">
        /// if successful
        /// </response>
        /// <response code="404">
        /// if any currency is not found
        /// </response>
        /// <response code="429">
        /// if not enough tokens
        /// </response>
        /// <response code="500">
        /// if unexpected error occurred
        /// </response>
        [HttpGet("{currencyCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyLoadBase))]
        public async Task<IActionResult> GetBase(string currencyCode)
        {
            var stringResponce = await _caller.CallAsync($"latest?currencies={currencyCode}&base_currency={_settings.baseCurrency}");
            ApiResponse castResponse = JsonSerializer.Deserialize<ApiResponse>(stringResponce);
            CurrencyLoadBase body = new(castResponse, _settings.currencyRoundCount);
            return Ok(body);
        }


        /// <summary>
        /// Get historical exchange rate of specified currency towards base one
        /// </summary>
        /// <param name="currencyCode">specified currency code</param>
        /// <param name="date">specified time [format(YYYY-MM-DD)]</param>
        /// <response code="200">
        /// if successful
        /// </response>
        /// <response code="404">
        /// if any currency is not found
        /// </response>
        /// <response code="429">
        /// if not enough tokens
        /// </response>
        /// <response code="500">
        /// if unexpected error occurred
        /// </response>
        [HttpGet("{currencyCode}/{date}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyLoadWDate))]
        public async Task<IActionResult> GetBase(string currencyCode, string date)
        {
            var stringResponce = await _caller.CallAsync($"historical?currencies={currencyCode}&date={date}&base_currency={_settings.baseCurrency}");
            ApiResponse castResponse = JsonSerializer.Deserialize<ApiResponse>(stringResponce);
            CurrencyLoadWDate body = new(castResponse, _settings.currencyRoundCount);
            return Ok(body);
        }


        /// <summary>
        /// Get default settings and token quota
        /// </summary>
        /// <response code="200">
        /// if successful
        /// </response>
        /// <response code="500">
        /// if unexpected error occurred
        /// </response>
        [HttpGet("/settings")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiStatus))]
        public async Task<IActionResult> GetSettings()
        {
            StatusResponce statusModel = JsonSerializer.Deserialize<StatusResponce>(await _caller.CallAsync("status", false));
            int requestLimit = statusModel.quotas.month.total;
            int requestCount = statusModel.quotas.month.used;

            ApiStatus body = new ApiStatus()
            {
                baseCurrency = _settings.baseCurrency,
                defaultCurrency = _settings.defaultCurrency,
                requestCount = requestCount,
                requestLimit = requestLimit,
                currencyRoundCount = _settings.currencyRoundCount
            };

            return Ok(body);
        }
    }
}
