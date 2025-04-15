using Common.Models;
using InternalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InternalApi.Services
{
    /// <summary>
    /// service to operate exchange rate cache
    /// </summary>
    public class CacheService
    {
        private readonly CacheDBContext _dbContext;
        public CacheService(CacheDBContext dBContext)
        {
            _dbContext = dBContext;
        }
        /// <summary>
        /// writes data to cache
        /// </summary>
        /// <param name="data">rates themselves</param>
        /// <param name="time">time to write as 'actuality timestamp'</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task WriteToCacheAsync(Currency[] data, DateTime time, CancellationToken cancellationToken)
        {
            CurrencyDB[] dbObjects = data.Select(cur => new CurrencyDB(cur, time)).ToArray();
            _dbContext.ExchangeRates.AddRange(dbObjects);
            await _dbContext.SaveChangesAsync(cancellationToken: cancellationToken);
            return;
        }
        /// <summary>
        /// get rate of epecified currency on specified time from cache
        /// </summary>
        /// <param name="ticksDateTime">desired time</param>
        /// <param name="currencyCode">desired currency</param>
        /// <param name="satisfactoryDelay">maximum time passed up to desired time</param>
        /// <param name="baseCurrencyCode">base currency</param>
        /// <param name="cancellationToken"></param>
        /// <returns>null if not found, rate if found</returns>
        public async Task<CurrencyDTO?> GetFromCacheAsync(DateTime ticksDateTime, CurrencyType currencyCode, TimeSpan satisfactoryDelay,
            CurrencyType baseCurrencyCode, CancellationToken cancellationToken)
        {
            DateTime borderEligible = ticksDateTime - satisfactoryDelay;
            var rates = _dbContext.ExchangeRates
                .Where(entry => entry.ActualityTime <= ticksDateTime.ToUniversalTime() && entry.ActualityTime > borderEligible.ToUniversalTime())
                .OrderByDescending(entry => entry.ActualityTime)
                .AsNoTracking();
            CurrencyDB[] pairRates = await rates.Where(rate => rate.Code == baseCurrencyCode || rate.Code == currencyCode)
                .ToArrayAsync(cancellationToken: cancellationToken);
            if (currencyCode == baseCurrencyCode)
            {
                return new CurrencyDTO()
                {
                    CurrencyType = currencyCode,
                    Value = 1
                };
            }
            else if (pairRates.Length < 2)
            {
                return null;
            }
            CurrencyDTO currency = new()
            {
                CurrencyType = currencyCode,
                Value = pairRates.Where(rate => rate.Code == currencyCode).First().ExchangeRate
                / pairRates.Where(rate => rate.Code == baseCurrencyCode).First().ExchangeRate
            };
            return currency;
        }
    }
}
