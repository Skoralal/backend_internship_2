using Common.Models;

namespace Fuse8.BackendInternship.PublicApi.Models
{
    public class FavoriteRateDBObject
    {
        public Guid Id { get; init; }
        public string Name { get; set; }
        public CurrencyType SelectedCurrencyType { get; set; }
        public CurrencyType BaseCurrencyType { get; set; }
    }
}
