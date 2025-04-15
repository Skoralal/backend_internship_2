using System.Text.Json.Serialization;

namespace InternalApi.Models
{
    // StatusResponce myDeserializedClass = JsonConvert.DeserializeObject<StatusResponce>(myJsonResponse);
    /// <summary>
    /// monthly quota
    /// </summary>
    public class Month
    {
        [JsonPropertyName("remaining")]
        public required int Remaining { get; set; }
    }
    /// <summary>
    /// different quotas
    /// </summary>
    public class Quotas
    {
        [JsonPropertyName("month")]
        public required Month Month { get; set; }
    }
    /// <summary>
    /// how API sends status information
    /// </summary>
    public class ApiStatusResponse
    {
        [JsonPropertyName("quotas")]
        public required Quotas Quotas { get; set; }
    }


}
