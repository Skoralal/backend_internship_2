using System.Text.Json;
using Common.Models;
using InternalApi.Contracts;
using InternalApi.Models;
using InternalApi.Models.Exceptions;
using Microsoft.Extensions.Options;

namespace InternalApi.Services
{
    public class ExternalCallerService:IExternalCallerService, ICachedCurrencyAPI, ICurrencyAPI
    {
        private readonly HttpClient _httpClient;
        private readonly IOptionsSnapshot<AppOptions> _appOptions;
        private readonly IOptionsSnapshot<NetOptions> _netOptions;
        private readonly CacheService _cacheService;
        private readonly long _dayInTicks = TimeSpan.FromHours(24).Ticks;
        public ExternalCallerService(HttpClient httpClient, IOptionsSnapshot<AppOptions> settings, CacheService cacheService, IOptionsSnapshot<NetOptions>netOptions)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(netOptions.Value.BaseURL);
            _httpClient.DefaultRequestHeaders.Add("apikey", netOptions.Value.ApiKey);
            _appOptions = settings;
            _cacheService = cacheService;
        }
        public async Task<string> CallAsync(string uri, bool usesTokens=true)
        {
            if (usesTokens)
            {
                if(!await HasTokens())
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

        public async Task<CurrenciesOnDate> GetAllCurrenciesOnDateAsync(string baseCurrency, DateOnly date, CancellationToken cancellationToken)
        {
            ApiResponse apiResponse = await GetApiResponseAsync($"historical?&date={date}&base_currency={_appOptions.Value.BaseCurrency}");
            CurrenciesOnDate output = new(apiResponse);
            _cacheService.WriteToCache(output.Currencies, output.LastUpdatedAt);
            return output;
        }

        public async Task<Currency[]> GetAllCurrentCurrenciesAsync(string baseCurrency, CancellationToken cancellationToken)
        {
            ApiResponse apiResponse = await GetApiResponseAsync($"latest?base_currency={_appOptions.Value.BaseCurrency}");
            Currency[] output = apiResponse.Data.Values.Select(x=>new Currency(x.Code, x.Value)).ToArray();
            _cacheService.WriteToCache(output, DateTime.UtcNow);
            return output;
        }

        public async Task<CurrencyDTO> GetCurrencyOnDateAsync(CurrencyType currencyType, DateOnly date, CancellationToken cancellationToken)
        {
            CurrencyDTO currencyDTO;
            if(!_cacheService.GetFromCache(date.ToDateTime(TimeOnly.MaxValue).Ticks, currencyType, _dayInTicks, out currencyDTO))
            {
                var allRates = await GetAllCurrenciesOnDateAsync(_appOptions.Value.BaseCurrency, date, cancellationToken);
                var currencyCodeString = currencyType.ToString();
                currencyDTO = allRates.Currencies
                    .Where(currency => currency.Code == currencyCodeString)
                    .Select(x=>new CurrencyDTO()
                {
                        CurrencyType = currencyType,
                        Value = x.Value,
                }).First();
            }
            return currencyDTO;
        }

        public async Task<CurrencyDTO> GetCurrentCurrencyAsync(CurrencyType currencyType, CancellationToken cancellationToken)
        {
            CurrencyDTO currencyDTO;
            if (!_cacheService.GetFromCache( DateTime.UtcNow.Ticks, currencyType, TimeSpan.FromHours(_appOptions.Value.CacheExpirationTimeHours).Ticks,
                out currencyDTO))
            {
                var allRates = await GetAllCurrentCurrenciesAsync(_appOptions.Value.BaseCurrency, cancellationToken);
                var currencyCodeString = currencyType.ToString();
                currencyDTO = allRates
                    .Where(currency => currency.Code == currencyCodeString)
                    .Select(x => new CurrencyDTO()
                    {
                        CurrencyType = currencyType,
                        Value = x.Value,
                    }).First();
            }
            return currencyDTO;
        }

        private async Task<ApiResponse> GetApiResponseAsync(string URL)
        {
            var json = await CallAsync(URL);
            return JsonSerializer.Deserialize<ApiResponse>(json);
        }
        public async Task<bool> HasTokens()
        {
            var jsonContent = await CallAsync("status", false);
            ApiStatusResponse statusModel = JsonSerializer.Deserialize<ApiStatusResponse>(jsonContent);
            if (statusModel.Quotas.Month.Remaining <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
