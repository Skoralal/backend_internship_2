using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models
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

    public class StatusResponse
    {
        [JsonPropertyName("quotas")]
        public Quotas Quotas { get; set; }
    }


}
