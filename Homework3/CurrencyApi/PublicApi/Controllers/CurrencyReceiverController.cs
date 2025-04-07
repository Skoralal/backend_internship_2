using System.Text.Json;
using Common.Models;
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
        private readonly DefaultSettings _settings;
        private readonly GrpcCurrencyService _grpcClient;
        public CurrencyReceiverController(IOptionsMonitor<DefaultSettings> settings, GrpcCurrencyService grpcClient)
        {
            _settings = settings.CurrentValue;
            _grpcClient = grpcClient;
        }
        /// <summary>
        /// Get exchange rate with default params
        /// </summary>
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
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyLoadBase))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<CurrencyLoadBase>> GetBaseAsync(CancellationToken cancellationToken)
        {
            CurrencyLoadBase body = await _grpcClient.GetCurrentCurrency(Enum.Parse<CurrencyType>(_settings.DefaultCurrency)
                , (byte)_settings.CurrencyRoundCount, cancellation: cancellationToken);
            return Ok(body);
        }


        /// <summary>
        /// Get exchange rate of specified currency towards base one
        /// </summary>
        /// <param name="currencyCode">specified currency code</param>
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
        [HttpGet("{currencyCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyLoadBase))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<CurrencyLoadBase>> GetSpecAsync([FromRoute] CurrencyType currencyCode, CancellationToken cancellationToken)
        {
            CurrencyLoadBase body = await _grpcClient.GetCurrentCurrency(currencyCode, (byte)_settings.CurrencyRoundCount, cancellation: cancellationToken);
            return Ok(body);
        }


        /// <summary>
        /// Get historical exchange rate of specified currency towards base one
        /// </summary>
        /// <param name="currencyCode">specified currency code</param>
        /// <param name="date">specified time [format(YYYY-MM-DD)]</param>
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
        [HttpGet("{currencyCode}/{date}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyLoadWDate))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<CurrencyLoadWDate>> GetDatedAsync([FromRoute] CurrencyType currencyCode,[FromRoute] DateOnly date, CancellationToken cancellationToken)
        {
            CurrencyLoadWDate body = await _grpcClient.GetHistoricalCurrency(currencyCode, date, (byte)_settings.CurrencyRoundCount, cancellation: cancellationToken);
            return Ok(body);
        }


        
    }
}
