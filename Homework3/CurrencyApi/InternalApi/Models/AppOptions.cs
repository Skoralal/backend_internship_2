namespace InternalApi.Models
{
    /// <summary>
    /// general app options
    /// </summary>
    public class AppOptions
    {
        /// <summary>
        /// against which the rate is calculated
        /// </summary>
        public required string BaseCurrency { get; set; }
        /// <summary>
        /// for how long is cache considered fresh in hours
        /// </summary>
        public long CacheExpirationTimeHours { get; set; }
    }
}