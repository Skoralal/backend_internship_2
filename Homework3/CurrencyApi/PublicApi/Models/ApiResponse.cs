using System.Text.Json.Serialization;
using Common.Models;

namespace Fuse8.BackendInternship.PublicApi.Models
{
    public class ApiResponse
    {
        [JsonPropertyName("data")]
            public Dictionary<string, Currency> Data { get; init; }
    }


    public class Currency
    {
        [JsonPropertyName("code")]

        public CurrencyType Code { get; init; }
        [JsonPropertyName("value")]

        public double Value { get; init; }
        public void Deconstruct(out CurrencyType code, out double value)
        {
            code = this.Code;
            value = this.Value;
        }
    }
}
