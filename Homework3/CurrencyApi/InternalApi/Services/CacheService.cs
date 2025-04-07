using System.Text.Json;
using Common.Models;
using InternalApi.Models;

namespace InternalApi.Services
{
    public class CacheService
    {
        private static readonly object _lock = new();
        private readonly string _cacheFolder;
        public CacheService()
        {
            _cacheFolder = Path.Combine(Directory.GetCurrentDirectory(), "cache");
        }
        public void WriteToCache(Currency[] data, DateTime time)
        {
            string jsonString = JsonSerializer.Serialize(data);
            lock (_lock)
            {
                File.WriteAllText(_cacheFolder + "/" + time.Ticks + ".json", jsonString);
            }

        }
        public bool GetFromCache(long ticksDateTime, CurrencyType currencyCode, long ticksSatisfactoryDelay, out CurrencyDTO currency)
        {
            currency = null;
            long borderEligible = ticksDateTime - ticksSatisfactoryDelay;
            long fileName = Directory.GetFiles(_cacheFolder)
                                          .Select(x => long.Parse(Path.GetFileNameWithoutExtension(x)))
                                          .Where(x=>x<=ticksDateTime).Where(x=>x>borderEligible)
                                          .OrderByDescending(x=>x)
                                          .FirstOrDefault(-1);
            if(fileName == -1)
            {
                return false;
            }
            string json = File.ReadAllText(_cacheFolder + "/" + fileName + ".json");
            Currency[] exchenges = JsonSerializer.Deserialize<Currency[]>(json);
            string currencyCodeString = currencyCode.ToString();
            currency = new(exchenges.First(x => x.Code == currencyCodeString));
            return true;
        }
    }
}
