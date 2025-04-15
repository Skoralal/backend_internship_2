using Common.Models;
using InternalApi.Models;
using InternalApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InternalApi.Controllers
{
    /// <summary>
    /// Check InternalApi information
    /// </summary>
    [Route("settings/")]
    public class StatusController : ControllerBase
    {
        private readonly HttpCallerService _caller;
        private readonly AppOptions _settings;
        public StatusController(HttpCallerService caller, IOptionsSnapshot<AppOptions> settings)
        {
            _caller = caller;
            _settings = settings.Value;
        }
        /// <summary>
        /// returns information about this api
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("getInfo")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiStatusResponse))]
        public async Task<ActionResult<ApiStatusResponse>> GetInfoAsync(CancellationToken cancellationToken)
        {
            var status = new GrpcStatusResponse()
            {
                BaseCurrency = Enum.Parse<CurrencyType>(_settings.BaseCurrency, true),
                HasRequests = await _caller.HasTokens(cancellationToken),
            };
            return Ok(status);
        }
    }
}
