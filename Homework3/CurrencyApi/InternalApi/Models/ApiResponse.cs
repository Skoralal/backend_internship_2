using System.Text.Json.Serialization;

namespace InternalApi.Models
{
    /// <summary>
    /// Model of how API gives exchange rates
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// meta information
        /// </summary>
        [JsonPropertyName("meta")]
        public required MetaInf Meta { get; set; }
        /// <summary>
        /// actual body [currency code: it's information]
        /// </summary>
        [JsonPropertyName("data")]
        public required Dictionary<string, Currency> Data { get; init; }
        public class MetaInf
        {
            [JsonPropertyName("last_updated_at")]
            public required string LastUpdatedAt { get; set; }
        }
    }
}
