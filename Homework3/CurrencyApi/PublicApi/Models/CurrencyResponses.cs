using System.Text.Json.Serialization;

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
        public string Code { get; set; }
        /// <summary>
        /// Exchange rate
        /// </summary>
        [JsonPropertyName("value")]
        public double Value { get; set; }
        public CurrencyLoadBase(ApiResponse response, int precision)
        {
            (this.Code, this.Value) = response.Data.Values.First();
            Value = Math.Round(Value, precision);
        }
    }
    /// <summary>
    /// Model of exchange rates between two currencies w/ date
    /// </summary>
    public record CurrencyLoadWDate:CurrencyLoadBase
    {
        /// <summary>
        /// Date of exchange rate [format(YYYY-MM-DD)]
        /// </summary>
        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }
        public CurrencyLoadWDate(ApiResponse response, int precision, DateOnly date) :base(response, precision)
        {
            Date = date;
        }
    }
}
