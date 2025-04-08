using System.Data.Common;
using Common.Models;
using Fuse8.BackendInternship.PublicApi.Models;
using Fuse8.BackendInternship.PublicApi.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Fuse8.BackendInternship.PublicApi.Services
{
    public class FavoriteExchangesService
    {
        private static UsersDBContext _dbContext;
        ILogger<UsersDBContext> _logger;
        public FavoriteExchangesService(UsersDBContext dbContext, ILogger<UsersDBContext> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task AddFavoriteAsync(FavoriteRateDBObject favoriteRate)
        {
            if(favoriteRate.SelectedCurrencyType == CurrencyType.NotSet || favoriteRate.SelectedCurrencyType == CurrencyType.NotSet)
            {
                throw new CrudOperationException("select both currencies");
            }
            if (!await IsNameUniqueAsync(favoriteRate.Name))
            {
                throw new CrudOperationException($"could not add {favoriteRate.Name} rate, another rate with the same name exists");
            }
            if(!await IsExchangeSetUniqueAsync(favoriteRate.SelectedCurrencyType, favoriteRate.BaseCurrencyType))
            {
                throw new CrudOperationException($"could not add {favoriteRate.Name} rate, another rate with the same currencies exists");
            }
            await _dbContext.AddAsync(favoriteRate);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Added new favorite rate, name {name}", favoriteRate.Name);
            return;
        }

        public async Task<FavoriteRateDBObject> GetFavoriteByNameAsync(string name)
        {
            FavoriteRateDBObject rate = await _dbContext.FavoriteExchanges.Where(line=>line.Name==name).AsNoTracking().FirstOrDefaultAsync();
            if (rate is null)
            {
                throw new CrudOperationException($"{name} rate not found");
            }
            return rate;
        }

        public async Task<FavoriteRateDBObject[]> GetAllFavoriteAsync()
        {
            FavoriteRateDBObject[] rates = await _dbContext.FavoriteExchanges.AsNoTracking().ToArrayAsync();
            if (rates.Length == 0)
            {
                throw new CrudOperationException($"There is no rates");
            }
            return rates;
        }

        public async Task ReplaceFavoriteByNameAsync(string name,string newName, CurrencyType? currency, CurrencyType? baseCurrency)
        {
            if (await IsNameUniqueAsync(name))
            {
                throw new CrudOperationException($"there is no rate named {name}");
            }
            if (name!=newName && !await IsNameUniqueAsync(newName))
            {
                throw new CrudOperationException($"could not edit {name} rate, another rate with the new name exists");
            }
            var rate = await _dbContext.FavoriteExchanges.Where(line => line.Name == name).FirstAsync();
            if(currency is null)
            {
                currency = rate.SelectedCurrencyType;
            }
            if (baseCurrency is null)
            {
                baseCurrency = rate.BaseCurrencyType;
            }
            if (!await IsExchangeSetUniqueAsync((CurrencyType)currency, 
                (CurrencyType)baseCurrency) && (rate.SelectedCurrencyType != currency && rate.BaseCurrencyType != baseCurrency))
            {
                throw new CrudOperationException($"could not edit {name} rate, another rate with the same currencies exists");
            }
            await _dbContext.FavoriteExchanges.Where(line => line.Name == name).ExecuteUpdateAsync(setters => setters
                .SetProperty(e => e.SelectedCurrencyType, currency).SetProperty(e => e.BaseCurrencyType, baseCurrency).SetProperty(e=>e.Name, newName));
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Edited favorite rate, name {oldName} --> {newName}; selected currency {oldCur} --> {newCur}; base currency {oldBase} --> {newBase}",
                name, newName, rate.BaseCurrencyType, baseCurrency, rate.BaseCurrencyType, baseCurrency);
            return;
        }

        public async Task DeleteFavoriteByNameAsync(string name)
        {
            var toDelete = await _dbContext.FavoriteExchanges.Where(line => line.Name == name).FirstOrDefaultAsync();
            if (toDelete is null)
            {
                throw new CrudOperationException($"{name} rate did not exist");
            }
            _dbContext.FavoriteExchanges.Remove(toDelete);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Deleted favorite rate, name {name}", name);
        }

        private async Task<bool> IsNameUniqueAsync(string name)
        {
            var evidence = await _dbContext.FavoriteExchanges.Where(x => x.Name == name).FirstOrDefaultAsync();
            if (evidence is null)
            {
                return true;
            }
            return false;
        }

        private async Task<bool> IsExchangeSetUniqueAsync(CurrencyType cur, CurrencyType baseCur)
        {
            var evidence = await _dbContext.FavoriteExchanges.Where(x=>x.BaseCurrencyType == baseCur).Where(x=>x.SelectedCurrencyType == cur).FirstOrDefaultAsync();
            if (evidence is null)
            {
                return true;
            }
            return false;
        }
    }
}
