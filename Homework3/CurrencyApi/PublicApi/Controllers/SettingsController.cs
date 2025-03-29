using Fuse8.BackendInternship.PublicApi.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Fuse8.BackendInternship.PublicApi.Services;
using Microsoft.Extensions.Options;

namespace Fuse8.BackendInternship.PublicApi.Controllers
{

    [Route("settings/")]
    public class SettingsController:ControllerBase
    {

        private readonly IExternalCallerService _caller;
        private readonly DefaultSettings _settings;
        public SettingsController(IExternalCallerService caller, IOptionsMonitor<DefaultSettings> settings)
        {
            _caller = caller;
            _settings = settings.CurrentValue;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MyStatus))]
        public async Task<IActionResult> GetSettings()
        {
            StatusResponse statusModel = JsonSerializer.Deserialize<StatusResponse>(await _caller.CallAsync("status", false));
            int requestLimit = statusModel.Quotas.Month.Total;
            int requestCount = statusModel.Quotas.Month.Used;

            MyStatus body = new MyStatus()
            {
                BaseCurrency = _settings.BaseCurrency,
                DefaultCurrency = _settings.DefaultCurrency,
                RequestCount = requestCount,
                RequestLimit = requestLimit,
                RoundingPrecision = _settings.CurrencyRoundCount
            };

            return Ok(body);
        }
    }
}
