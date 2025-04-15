namespace InternalApi.Models
{
    public class NetOptions
    {
        /// <summary>
        /// Currency API key - covert
        /// </summary>
        public required string ApiKey { get; set; }
        /// <summary>
        /// base url to call currency API - not covert
        /// </summary>
        public required string BaseURL { get; set; }
    }
}
