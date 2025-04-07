namespace InternalApi.Models
{
    public class AppOptions
    {
        public string DefaultCurrency { get; set; }
        public string BaseCurrency { get; set; }
        public long CacheExpirationTimeHours { get; set; }
    }
}