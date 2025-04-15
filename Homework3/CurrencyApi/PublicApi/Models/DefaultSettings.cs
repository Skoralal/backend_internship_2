namespace Fuse8.BackendInternship.PublicApi.Models
{
    public class DefaultSettings
    {
        /// <summary>
        /// default currency to be valued agains a base currency
        /// </summary>
        public required string DefaultCurrency { get; set; }
        /// <summary>
        /// precision of rounding an exchange rate
        /// </summary>
        public int CurrencyRoundCount { get; set; }
    }
}