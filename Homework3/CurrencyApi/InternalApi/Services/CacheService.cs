using System.Text.Json;
using System.Threading.Tasks;
using Common.Models;
using InternalApi.Models;
using InternalApi.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace InternalApi.Services
{
    public class CacheService
    {
        private static readonly object _lock = new();
        private readonly string _cacheFolder;
        CacheDBContext _dbContext;
        public CacheService(CacheDBContext dBContext)
        {
            _cacheFolder = Path.Combine(Directory.GetCurrentDirectory(), "cache");
            _dbContext = dBContext;
        }
        public async Task WriteToCache(Currency[] data, long time = -1)
        {
            if (time == -1)
            {
                time = DateTime.UtcNow.Ticks;
            }
            CurrencyDateBaseObject[] dbObjects = data.Select(cur => new CurrencyDateBaseObject(cur, time)).ToArray();
            _dbContext.ExchangeRates.AddRange(dbObjects);
            await _dbContext.SaveChangesAsync();
            return;
        }
        public async Task<CurrencyDTO?> GetFromCache(long ticksDateTime, CurrencyType currencyCode, long ticksSatisfactoryDelay,
            CurrencyType baseCurrencyCode = CurrencyType.USD)
        {
            long borderEligible = ticksDateTime - ticksSatisfactoryDelay;
            var rates = _dbContext.ExchangeRates.Where(entry => entry.ActualityTime < ticksDateTime)
                .Where(entry => entry.ActualityTime > borderEligible).OrderByDescending(entry => entry.ActualityTime)
                .AsNoTracking();

            var baseRate = await rates.Where(rate=>rate.Code==baseCurrencyCode.ToString()).FirstOrDefaultAsync();
            var neededRate = await rates.Where(rate=>rate.Code==currencyCode.ToString()).FirstOrDefaultAsync();
            if(baseRate is null && neededRate is null)
            {
                return null;
            }
            else if (baseRate is null ^ neededRate is null)
            {
                throw new CurrencyNotFoundException("no data for one of the currencies");
            }
            CurrencyDTO currency = new()
            {
                CurrencyType = currencyCode,
                Value = 1 / baseRate.ExchangeRate * neededRate.ExchangeRate
            };
            return currency;
        }
    }
}
