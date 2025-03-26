using System.Text.Json;
using Fuse8.BackendInternship.PublicApi.Models;
using Fuse8.BackendInternship.PublicApi.Models.Exceptions;
using Microsoft.Extensions.Options;

namespace Fuse8.BackendInternship.PublicApi.Services
{
    public class ExternalCallerService:IExternalCallerService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptionsMonitor<SecretSettings> _secretSettings;
        public ExternalCallerService(HttpClient httpClient, IOptionsMonitor<SecretSettings> secretSettings)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("apikey", secretSettings.CurrentValue.ApiKey);
        }
        public async Task<string> CallAsync(string uri, bool usesTokens=true)
        {
            if (usesTokens)
            {
                var jsonContent = await CallAsync("status", false);
                StatusResponce statusModel = JsonSerializer.Deserialize<StatusResponce>(jsonContent);
                if(statusModel.quotas.month.remaining == 0)
                {
                    throw new ApiRequestLimitException(nameof(CallAsync));
                }
            }
            var responce = await _httpClient.GetAsync(uri);
            if (responce.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await responce.Content.ReadAsStringAsync();
            }
            if (responce.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
            {
                throw new CurrencyNotFoundException();
            }

            throw new Exception();
        }
    }
}
