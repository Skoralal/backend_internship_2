using System.Text.Json;
using Fuse8.BackendInternship.PublicApi.Models;
using Fuse8.BackendInternship.PublicApi.Models.Exceptions;
using Microsoft.Extensions.Options;

namespace Fuse8.BackendInternship.PublicApi.Services
{
    public class ExternalCallerService:IExternalCallerService
    {
        private readonly HttpClient _httpClient;
        public ExternalCallerService(HttpClient httpClient, IOptionsSnapshot<DefaultSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.BaseURL);
            _httpClient.DefaultRequestHeaders.Add("apikey", settings.Value.ApiKey);
        }
        public async Task<string> CallAsync(string uri, bool usesTokens=true)
        {
            if (usesTokens)
            {
                var jsonContent = await CallAsync("status", false);
                StatusResponse statusModel = JsonSerializer.Deserialize<StatusResponse>(jsonContent);
                if(statusModel.Quotas.Month.Remaining <= 0)
                {
                    throw new ApiRequestLimitException(nameof(CallAsync));
                }
            }
            var response = await _httpClient.GetAsync(uri);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
            {
                var stringResponse = await response.Content.ReadAsStringAsync();
                if (stringResponse.Contains("The selected currencies is invalid"))
                {
                    throw new CurrencyNotFoundException();
                };
            }

            throw new UnexpectedAPIResponseException(response.StatusCode);
        }
    }
}
