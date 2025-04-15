using System.Text.Json.Serialization;
using Common.Models;

namespace Fuse8.BackendInternship.PublicApi.Models
{
    /// <summary>
    /// Model exchange rates between two currencies w/o date
    /// </summary>
    public record CurrencyLoadBase
    {
        /// <summary>
        /// ISO 4217 currency code
        /// </summary>
        [JsonPropertyName("code")]
        public CurrencyType Code { get; set; }
        /// <summary>
        /// Exchange rate
        /// </summary>
        [JsonPropertyName("value")]
        public double Value { get; set; }
        public CurrencyLoadBase() { }
    }
    /// <summary>
    /// Model of exchange rates between two currencies w/ date
    /// </summary>
    public record CurrencyLoadWDate : CurrencyLoadBase
    {
        /// <summary>
        /// Date of exchange rate [format(YYYY-MM-DD)]
        /// </summary>
        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }
        public CurrencyLoadWDate() { }
    }
}
