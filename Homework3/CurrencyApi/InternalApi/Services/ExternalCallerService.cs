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
        private readonly CurrencyType _baseDefault = CurrencyType.USD;
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

        public async Task<CurrenciesOnDate> GetAllCurrenciesOnDateAsync(CurrencyType baseCurrency, DateOnly date, CancellationToken cancellationToken)
        {
            ApiResponse apiResponse = await GetApiResponseAsync($"historical?&date={date}&base_currency={_baseDefault}");
            CurrenciesOnDate output = new(apiResponse);
            await _cacheService.WriteToCache(output.Currencies, output.LastUpdatedAt.Ticks);
            return output;
        }

        public async Task<Currency[]> GetAllCurrentCurrenciesAsync(CurrencyType baseCurrency, CancellationToken cancellationToken)
        {
            ApiResponse apiResponse = await GetApiResponseAsync($"latest?base_currency={_baseDefault}");
            Currency[] output = apiResponse.Data.Values.Select(x=>new Currency(x.Code, x.Value)).ToArray();
            await _cacheService.WriteToCache(output, DateTime.UtcNow.Ticks);
            return output;
        }

        public async Task<CurrencyDTO> GetCurrencyOnDateAsync(CurrencyType currencyType,CurrencyType baseType, DateOnly date, CancellationToken cancellationToken)
        {
            CurrencyDTO? currencyDTO = await _cacheService.GetFromCache(date.ToDateTime(TimeOnly.MaxValue).Ticks, currencyType, _dayInTicks, baseType);
            if(currencyDTO is null)
            {
                await GetAllCurrenciesOnDateAsync(baseType, date, cancellationToken);
                currencyDTO = await _cacheService.GetFromCache(date.ToDateTime(TimeOnly.MaxValue).Ticks, currencyType, _dayInTicks, baseType);
            }
            return currencyDTO;
        }

        public async Task<CurrencyDTO> GetCurrentCurrencyAsync(CurrencyType currencyType, CurrencyType baseType, CancellationToken cancellationToken)
        {
            CurrencyDTO? currencyDTO = await _cacheService.GetFromCache(DateTime.UtcNow.Ticks, currencyType, TimeSpan.FromHours(_appOptions.Value.CacheExpirationTimeHours).Ticks, baseType);
            if (currencyDTO is null)
            {
                await GetAllCurrentCurrenciesAsync(baseType, cancellationToken);
                currencyDTO = await _cacheService.GetFromCache(DateTime.UtcNow.Ticks, currencyType, TimeSpan.FromHours(_appOptions.Value.CacheExpirationTimeHours).Ticks, baseType);
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
