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

        private readonly DefaultSettings _settings;
        private readonly GrpcCurrencyService _grpcClient;
        public SettingsController(IOptionsMonitor<DefaultSettings> settings, GrpcCurrencyService client)
        {
            _settings = settings.CurrentValue;
            _grpcClient = client;
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
            var internalSettings = await _grpcClient.GetStatus();
            MyStatus body = new MyStatus()
            {
                BaseCurrency = internalSettings.BaseCurrency.ToString().ToUpper(),
                DefaultCurrency = _settings.DefaultCurrency,
                RoundingPrecision = _settings.CurrencyRoundCount,
                HasTokens = internalSettings.HasRequests
            };

            return Ok(body);
        }
    }
}
