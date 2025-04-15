using Common.Models;
using InternalApi.Models;
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
    public class CurrencyReceiverController : ControllerBase
    {
        private readonly CurrencyRequestHandlerService _caller;
        private readonly AppOptions _settings;
        public CurrencyReceiverController(CurrencyRequestHandlerService caller, IOptionsMonitor<AppOptions> settings)
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
        [HttpGet("{currencyCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<CurrencyDTO>> GetBaseAsync([FromRoute] CurrencyType currencyCode, CancellationToken cancellationToken)
        {
            var currency = await _caller.GetCurrentCurrencyAsync(currencyCode, CurrencyType.USD, cancellationToken);
            return Ok(currency);
        }


        /// <summary>
        /// Get exchange rate of specified currency towards base one
        /// </summary>
        /// <param name="currencyCode">specified currency code</param>
        /// <param name="baseCurrencyCode">specified base currency code</param>
        /// <param name="cancellationToken">cancellation token</param>
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
        [HttpGet("{currencyCode}/{baseCurrencyCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<CurrencyDTO>> GetBase([FromRoute] CurrencyType currencyCode,
            [FromRoute] CurrencyType baseCurrencyCode, CancellationToken cancellationToken)
        {
            var currency = await _caller.GetCurrentCurrencyAsync(currencyCode, baseCurrencyCode, cancellationToken);
            return Ok(currency);
        }


        /// <summary>
        /// Get historical exchange rate of specified currency towards base one
        /// </summary>
        /// <param name="currencyCode">specified currency code</param>
        /// <param name="date">specified time [format(YYYY-MM-DD)]</param>
        /// <param name="baseCurrencyCode">specified base currency code</param>
        /// <param name="cancellationToken">cancellation token</param>
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
        [HttpGet("{currencyCode}/{baseCurrencyCode}/{date}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<CurrencyDTO>> GetBase([FromRoute] CurrencyType currencyCode, [FromRoute] CurrencyType baseCurrencyCode,
            [FromRoute] DateOnly date, CancellationToken cancellationToken)
        {
            var currency = await _caller.GetCurrencyOnDateAsync(currencyCode, baseCurrencyCode, date.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc), cancellationToken);
            return Ok(currency);
        }



    }
}
