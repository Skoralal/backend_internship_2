using Common.Models;

namespace Fuse8.BackendInternship.PublicApi.Models
{
    public class FavoriteRateRequestModel
    {
        /// <summary>
        /// desired currency
        /// </summary>
        public CurrencyType Currency { get; set; }
        /// <summary>
        /// base currency
        /// </summary>
        public CurrencyType BaseCurrency { get; set; }
        /// <summary>
        /// name of the rate
        /// </summary>
        public required string Name { get; set; }
    }
}
