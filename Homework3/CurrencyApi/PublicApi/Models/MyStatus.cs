using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models
{
    /// <summary>
    /// Model of returning data for the end user
    /// </summary>
    public record MyStatus
    {
        /// <summary>
        /// Default currency (how much of this for 1 base currency)
        /// </summary>
        [JsonPropertyName("defaultCurrency")]
        public string DefaultCurrency { get; set; } = "";
        /// <summary>
        /// Base currency (how much of default currency for 1 of this)
        /// </summary>
        [JsonPropertyName("baseCurrency")]
        public string BaseCurrency { get; set; } = "";
        /// <summary>
        /// Overall number of calls
        /// </summary>
        [JsonPropertyName("requestLimit")]
        public int RequestLimit { get; set; } = 0;
        /// <summary>
        /// Number of used calls
        /// </summary>
        [JsonPropertyName("requestCount")]
        public int RequestCount { get; set; } = 0;
        /// <summary>
        /// Precision of rounding exchange rates
        /// </summary>
        [JsonPropertyName("currencyRoundCount")]
        public int RoundingPrecision { get; set; } = 0;
    }
}
