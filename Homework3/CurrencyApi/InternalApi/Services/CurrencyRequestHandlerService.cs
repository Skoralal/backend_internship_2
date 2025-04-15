using System.Text.Json;
using Common.Models;
using InternalApi.Models;
using Microsoft.Extensions.Options;

namespace InternalApi.Services
{
    /// <summary>
    /// service to connect cache and external api with controllers
    /// </summary>
    public class CurrencyRequestHandlerService
    {
        private readonly IOptionsSnapshot<AppOptions> _appOptions;
        private readonly CacheService _cacheService;
        private readonly TimeSpan _dayInTicks = TimeSpan.FromHours(24);
        private readonly CurrencyType _baseDefault = CurrencyType.USD;
        private readonly HttpCallerService _httpCallerService;
        public CurrencyRequestHandlerService(IOptionsSnapshot<AppOptions> settings, CacheService cacheService,
            HttpCallerService httpCallerService)
        {
            _appOptions = settings;
            _cacheService = cacheService;
            _httpCallerService = httpCallerService;
        }
        /// <summary>
        /// get exchange rate on supplied date from cache, if not found, cahce is refreshed and the rate is returned
        /// </summary>
        /// <param name="currencyType">desired currency</param>
        /// <param name="baseType">base currency</param>
        /// <param name="time">desired time</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CurrencyDTO> GetCurrencyOnDateAsync(CurrencyType currencyType, CurrencyType baseType, DateTime time, CancellationToken cancellationToken)
        {
            CurrencyDTO? currencyDTO = await _cacheService.GetFromCacheAsync(time, currencyType, _dayInTicks, baseType, cancellationToken: cancellationToken);
            if (currencyDTO is null)
            {
                await RefrechCacheOnDateAsync(baseType, DateOnly.FromDateTime(time), cancellationToken);
                currencyDTO = await _cacheService.GetFromCacheAsync(time, currencyType, _dayInTicks, baseType, cancellationToken: cancellationToken);
            }
            return currencyDTO!;
        }
        /// <summary>
        /// get latest exchange rate from cache, if not found, cahce is refreshed and the rate is returned
        /// </summary>
        /// <param name="currencyType">desired currency</param>
        /// <param name="baseType">base currency</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CurrencyDTO> GetCurrentCurrencyAsync(CurrencyType currencyType, CurrencyType baseType, CancellationToken cancellationToken)
        {
            CurrencyDTO? currencyDTO = await _cacheService.GetFromCacheAsync(DateTime.UtcNow, currencyType, TimeSpan.FromHours(_appOptions.Value.CacheExpirationTimeHours), baseType, cancellationToken: cancellationToken);
            if (currencyDTO is null)
            {
                await RefreshLatestCacheAsync(baseType, cancellationToken);
                currencyDTO = await _cacheService.GetFromCacheAsync(DateTime.UtcNow, currencyType,
                    TimeSpan.FromHours(_appOptions.Value.CacheExpirationTimeHours), baseType, cancellationToken: cancellationToken);
            }
            return currencyDTO!;
        }

        private async Task<CurrenciesOnDate> RefrechCacheOnDateAsync(CurrencyType baseCurrency, DateOnly date, CancellationToken cancellationToken)
        {
            ApiResponse apiResponse = await GetApiResponseAsync($"historical?&date={date}&base_currency={_baseDefault}", cancellationToken);
            CurrenciesOnDate output = new(apiResponse);
            await _cacheService.WriteToCacheAsync(output.Currencies, output.LastUpdatedAt, cancellationToken: cancellationToken);
            return output;
        }

        private async Task<Currency[]> RefreshLatestCacheAsync(CurrencyType baseCurrency, CancellationToken cancellationToken)
        {
            ApiResponse apiResponse = await GetApiResponseAsync($"latest?base_currency={_baseDefault}", cancellationToken);
            Currency[] output = apiResponse.Data.Values.Select(x => new Currency(x.Code, x.Value)).ToArray();
            await _cacheService.WriteToCacheAsync(output, DateTime.UtcNow, cancellationToken: cancellationToken);
            return output;
        }

        private async Task<ApiResponse> GetApiResponseAsync(string URL, CancellationToken cancellationToken)
        {
            var json = await _httpCallerService.CallAsync(URL, cancellationToken);
            return JsonSerializer.Deserialize<ApiResponse>(json) ?? throw new ArgumentNullException("Failed to deserialize an api response");
        }
    }
}
