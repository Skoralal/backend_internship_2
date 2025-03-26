namespace Fuse8.BackendInternship.PublicApi.Models
{
    /// <summary>
    /// Model of returning data for the end user
    /// </summary>
    public record ApiStatus
    {
        /// <summary>
        /// Default currency (how much of this for 1 base currency)
        /// </summary>
        public string defaultCurrency { get; set; } = "";
        /// <summary>
        /// Base currency (how much of default currency for 1 of this)
        /// </summary>
        public string baseCurrency { get; set; } = "";
        /// <summary>
        /// Overall number of calls
        /// </summary>
        public int requestLimit { get; set; } = 0;
        /// <summary>
        /// Number of used calls
        /// </summary>
        public int requestCount { get; set; } = 0;
        /// <summary>
        /// Precision of rounding exchange rates
        /// </summary>
        public int currencyRoundCount { get; set; } = 0;
    }
}
