using InternalApi.Contracts;
using InternalApi.Models;

namespace InternalApi.Services
{
    public class CachedCurrencyAPI : ICachedCurrencyAPI
    {
        public Task<CurrencyDTO> GetCurrencyOnDateAsync(CurrencyType currencyType, DateOnly date, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<CurrencyDTO> GetCurrentCurrencyAsync(CurrencyType currencyType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
