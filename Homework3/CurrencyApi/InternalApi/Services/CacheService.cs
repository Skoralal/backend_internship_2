using System.Text.Json;
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

            // Ensure the directory exists
            if (!Directory.Exists(_cacheFolder))
            {
                Directory.CreateDirectory(_cacheFolder);
            }
            string jsonString = JsonSerializer.Serialize(data);
            lock (_lock)
            {
                File.WriteAllText(_cacheFolder + "/" + time.Ticks + ".json", jsonString);
            }

        }
        public bool GetFromCache(out CurrencyDTO currency, long ticksDateTime, CurrencyType currencyCode, long ticksSatisfactoryDelay)
        {
            currency = null;
            long minimumEligible = ticksDateTime - ticksSatisfactoryDelay;
            long fileName = Directory.GetFiles(_cacheFolder)
                                          .Select(x => long.Parse(Path.GetFileNameWithoutExtension(x)))
                                          .Where(x=>x<=ticksDateTime).Where(x=>x>minimumEligible)
                                          .OrderByDescending(x=>x)
                                          .FirstOrDefault(-1);
            if(fileName == -1)
            {
                return false;
            }
            string json = File.ReadAllText(_cacheFolder + "/" + fileName + ".json");
            Currency[] exchenges = JsonSerializer.Deserialize<Currency[]>(json);
            string currencyCodeString = currencyCode.ToString();
            currency = new(exchenges.Where(x => x.Code == currencyCodeString).First());
            return true;
        }
    }
}
