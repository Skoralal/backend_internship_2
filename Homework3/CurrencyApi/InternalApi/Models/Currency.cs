using System.Text.Json.Serialization;

namespace InternalApi.Models
{
    /// <summary>
    /// Курс валюты
    /// </summary>
    public record Currency
    {
        /// <summary>
        /// code of the currency
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }
        /// <summary>
        /// exchange rate
        /// </summary>
        [JsonPropertyName("value")]
        public decimal Value { get; set; }
        public Currency(string code, decimal value)
        {
            Code = code;
            Value = value;
        }
    };
}
