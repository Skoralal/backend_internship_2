using System.Text.Json.Serialization;

namespace InternalApi.Models
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
        public decimal Value { get; set; }
        public CurrencyLoadBase(ApiResponse response, int precision)
        {
            Code = response.Data.First().Value.Code;
            Value = response.Data.First().Value.Value;
            Value = Math.Round(Value, precision);
        }

    }
    /// <summary>
    /// Model of exchange rates between two currencies w/ date
    /// </summary>
    public record CurrencyLoadWDate : CurrencyLoadBase
    {
        /// <summary>
        /// Date the rate was last actual
        /// </summary>
        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }
        public CurrencyLoadWDate(ApiResponse response, int precision, DateOnly date) : base(response, precision)
        {
            Date = date;
        }
    }
}
