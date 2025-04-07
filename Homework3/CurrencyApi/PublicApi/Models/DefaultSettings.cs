namespace Fuse8.BackendInternship.PublicApi.Models
{
    public class DefaultSettings
    {
        public string DefaultCurrency { get; set; }
        public string BaseCurrency { get; set; }
        public int CurrencyRoundCount { get; set; }
        public string ApiKey { get; set; }
        public string BaseURL { get; set; }
        public string GrpcURL { get; set; }
    }
}