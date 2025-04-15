using Common.Models;

namespace Fuse8.BackendInternship.PublicApi.Models
{
    public class FavoriteRateDB
    {
        public Guid Id { get; init; }
        /// <summary>
        /// name of a rate
        /// </summary>
        public required string Name { get; set; }
        /// <summary>
        /// desired currency
        /// </summary>
        public CurrencyType SelectedCurrencyType { get; set; }
        /// <summary>
        /// base currency
        /// </summary>
        public CurrencyType BaseCurrencyType { get; set; }
    }
}
