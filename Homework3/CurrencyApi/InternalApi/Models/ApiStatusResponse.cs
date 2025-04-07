using System.Text.Json.Serialization;

namespace InternalApi.Models
{
    // StatusResponce myDeserializedClass = JsonConvert.DeserializeObject<StatusResponce>(myJsonResponse);

    public class Month
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("used")]
        public int Used { get; set; }
        [JsonPropertyName("remaining")]
        public int Remaining { get; set; }
    }

    public class Quotas
    {
        [JsonPropertyName("month")]
        public Month Month { get; set; }
    }

    public class ApiStatusResponse
    {
        [JsonPropertyName("quotas")]
        public Quotas Quotas { get; set; }
    }


}
