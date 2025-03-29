using System.Text.Json.Serialization;

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

        public string Code { get; init; }
        [JsonPropertyName("value")]

        public double Value { get; init; }
        public void Deconstruct(out string code, out double value)
        {
            code = this.Code;
            value = this.Value;
        }
    }
}
