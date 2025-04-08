using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models
{
    /// <summary>
    /// Model of returning data for the end user
    /// </summary>
    public record MyStatus
    {
        /// <summary>
        /// Do you have tokens to make requests
        /// </summary>
        [JsonPropertyName("newRequestsAvailable")]
        public bool HasTokens { get; set; }
    }
}
