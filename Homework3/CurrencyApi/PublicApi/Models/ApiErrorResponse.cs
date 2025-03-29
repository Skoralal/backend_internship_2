using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models
{
    public class ApiErrorResponse
    {
        [JsonPropertyName("message")]
        public string Message {  get; init; }
        [JsonPropertyName("errors")]
        public Dictionary<string, string[]> Errors { get; init; }
    }
}
