using System.Text.Json.Serialization;

namespace InternalApi.Models
{
    public class ApiResponse
    {
        [JsonPropertyName("meta")]
        public MetaInf Meta { get; set; }
        [JsonPropertyName("data")]
        public Dictionary<string, Currency> Data { get; init; }
        public class MetaInf
        {
            [JsonPropertyName("last_updated_at")]
            public string LastUpdatedAt { get; set; }
        }
    }
}
