using System.Text.Json;
using System.Threading;
using InternalApi.Models;
using InternalApi.Models.Exceptions;
using InternalApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InternalApi.Controllers
{
    /// <summary>
    /// Calls CurrencyAPI
    /// </summary>
    [ApiController]
    [Route("currency/")]
    public class CurrencyReceiverController:ControllerBase
    {
        private readonly ExternalCallerService _caller;
        private readonly DefaultSettings _settings;
        public CurrencyReceiverController(ExternalCallerService caller, IOptionsMonitor<DefaultSettings> settings)
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetBaseAsync(CancellationToken cancellationToken)
        {
            CurrencyType currencyType = default;
            if (!Enum.TryParse(_settings.DefaultCurrency.ToUpper(), out currencyType))
            {
                throw new CurrencyNotFoundException();
            };
            var currency = await _caller.GetCurrentCurrencyAsync(currencyType, cancellationToken);
            return Ok(currency);
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetBase([FromRoute]string currencyCode, CancellationToken cancellationToken)
        {
            CurrencyType currencyType = default;
            if(!Enum.TryParse(currencyCode.ToUpper(), out currencyType))
            {
                throw new CurrencyNotFoundException();
            };
            var currency = await _caller.GetCurrentCurrencyAsync(currencyType, cancellationToken);
            return Ok(currency);
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetBase([FromRoute] string currencyCode,[FromRoute] DateOnly date, CancellationToken cancellationToken)
        {
            CurrencyType currencyType = default;
            if (!Enum.TryParse(currencyCode.ToUpper(), out currencyType))
            {
                throw new CurrencyNotFoundException();
            };
            var currency = await _caller.GetCurrencyOnDateAsync(currencyType, date, cancellationToken);
            return Ok(currency);
        }


        
    }
}
