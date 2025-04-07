using System.Text.Json.Serialization;

namespace InternalApi.Models
{
    /// <summary>
    /// Курс валюты
    /// </summary>
    /// <param name="Code">Код валюты</param>
    /// <param name="Value">Значение курса валют, относительно базовой валюты</param>
    public record Currency
    {
        [JsonPropertyName("code")]
        public string Code {  get; set; }
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
