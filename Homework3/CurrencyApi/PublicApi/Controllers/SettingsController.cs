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
        public async Task<ActionResult<MyStatus>> GetSettings(CancellationToken cancellation)
        {
            var internalSettings = await _grpcClient.GetStatus(cancellation);
            MyStatus body = new MyStatus()
            {
                HasTokens = internalSettings.HasRequests
            };

            return Ok(body);
        }
    }
}
