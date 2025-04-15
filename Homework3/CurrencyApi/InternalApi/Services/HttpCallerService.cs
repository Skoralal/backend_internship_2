using System.Text.Json;
using Common.Models.Exceptions;
using InternalApi.Models;
using Microsoft.Extensions.Options;

namespace InternalApi.Services
{
    /// <summary>
    /// service to execute calls to the API
    /// </summary>
    public class HttpCallerService
    {
        private readonly HttpClient _httpClient;
        public HttpCallerService(HttpClient httpClient, IOptionsSnapshot<NetOptions> netOptions)
        {
            _httpClient = httpClient;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(netOptions.Value.BaseURL);
            _httpClient.DefaultRequestHeaders.Add("apikey", netOptions.Value.ApiKey);
        }
        /// <summary>
        /// calls supplied adress
        /// </summary>
        /// <param name="uri">adress to call</param>
        /// <param name="cancellationToken"></param>
        /// <param name="usesTokens">if the call uses tokens</param>
        /// <returns>response as a string</returns>
        /// <exception cref="ApiRequestLimitException">no tokens</exception>
        /// <exception cref="CurrencyNotFoundException">some of the supplied currencies are invalid</exception>
        /// <exception cref="UnexpectedAPIResponseException">unknown error</exception>
        public async Task<string> CallAsync(string uri, CancellationToken cancellationToken, bool usesTokens = true)
        {
            if (usesTokens)
            {
                if (!await HasTokens(cancellationToken))
                {
                    throw new ApiRequestLimitException(nameof(CallAsync));
                }
            }
            var response = await _httpClient.GetAsync(uri, cancellationToken: cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
            {
                var stringResponse = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
                if (stringResponse.Contains("The selected currencies is invalid"))
                {
                    throw new CurrencyNotFoundException();
                }
                ;
            }

            throw new UnexpectedAPIResponseException(response.StatusCode);
        }

        public async Task<bool> HasTokens(CancellationToken cancellationToken)
        {
            var jsonContent = await CallAsync("status", cancellationToken: cancellationToken, false);
            ApiStatusResponse statusModel = JsonSerializer.Deserialize<ApiStatusResponse>(jsonContent)
                ?? throw new ArgumentNullException("Failed to deserialize a status model");
            if (statusModel.Quotas.Month.Remaining <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
