using Common.Models;
using Fuse8.BackendInternship.PublicApi.Models;
using Fuse8.BackendInternship.PublicApi.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Fuse8.BackendInternship.PublicApi.Services
{
    /// <summary>
    /// service to manipulate favorite exchanges persistent storage
    /// </summary>
    public class FavoriteExchangesService
    {
        private readonly UsersDBContext _dbContext;
        private readonly ILogger<FavoriteExchangesService> _logger;
        public FavoriteExchangesService(UsersDBContext dbContext, ILogger<FavoriteExchangesService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        /// <summary>
        /// add a favorite rate
        /// </summary>
        /// <param name="favoriteRate">model</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="CrudOperationException">if something went wrong during a crud operation, specifics included</exception>
        public async Task AddFavoriteAsync(FavoriteRateDB favoriteRate, CancellationToken cancellationToken)
        {
            if (favoriteRate.SelectedCurrencyType == CurrencyType.NotSet || favoriteRate.SelectedCurrencyType == CurrencyType.NotSet)
            {
                throw new CrudOperationException("Select both currencies");
            }
            if (!await IsNameUniqueAsync(favoriteRate.Name, cancellationToken: cancellationToken))
            {
                throw new CrudOperationException($"Could not add '{favoriteRate.Name}' rate, another rate with the same name exists");
            }
            if (!await IsExchangeSetUniqueAsync(favoriteRate.SelectedCurrencyType, favoriteRate.BaseCurrencyType, cancellationToken: cancellationToken))
            {
                throw new CrudOperationException($"Could not add '{favoriteRate.Name}' rate, another rate with the same currencies exists");
            }
            await _dbContext.AddAsync(favoriteRate, cancellationToken: cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("Added new favorite rate, name '{name}'", favoriteRate.Name);
            return;
        }
        /// <summary>
        /// get a favorite rate by name
        /// </summary>
        /// <param name="name">name of the rate to search</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="CrudOperationException">if something went wrong during a crud operation, specifics included</exception>
        public async Task<FavoriteRateDB> GetFavoriteByNameAsync(string name, CancellationToken cancellationToken)
        {
            FavoriteRateDB? rate = await _dbContext.FavoriteExchanges.Where(line => line.Name == name).AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (rate is null)
            {
                throw new CrudOperationException($"'{name}' rate not found");
            }
            return rate;
        }
        /// <summary>
        /// get all existing favorite rates
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<FavoriteRateDB[]> GetAllFavoriteAsync(CancellationToken cancellationToken)
        {
            FavoriteRateDB[] rates = await _dbContext.FavoriteExchanges.AsNoTracking().ToArrayAsync(cancellationToken: cancellationToken);
            return rates;
        }
        /// <summary>
        /// modify existing favorite rate
        /// </summary>
        /// <param name="name">name of the rate to be modified</param>
        /// <param name="model">new data</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="CrudOperationException">if something went wrong during a crud operation, specifics included</exception>
        public async Task ReplaceFavoriteByNameAsync(string name, FavoriteRateRequestModel model,
            CancellationToken cancellationToken)
        {
            if (await IsNameUniqueAsync(name, cancellationToken: cancellationToken))
            {
                throw new CrudOperationException($"There is no rate named '{name}'");
            }
            if (name != model.Name && !await IsNameUniqueAsync(model.Name, cancellationToken: cancellationToken))
            {
                throw new CrudOperationException($"Could not edit '{name}' rate, another rate with the new name exists");
            }
            var rate = await _dbContext.FavoriteExchanges.Where(line => line.Name == name).FirstAsync(cancellationToken: cancellationToken);
            if (model.Currency == CurrencyType.NotSet)
            {
                model.Currency = rate.SelectedCurrencyType;
            }
            if (model.BaseCurrency == CurrencyType.NotSet)
            {
                model.BaseCurrency = rate.BaseCurrencyType;
            }
            if (!await IsExchangeSetUniqueAsync(model.Currency,
                model.BaseCurrency, cancellationToken: cancellationToken)
                && (rate.SelectedCurrencyType != model.Currency && rate.BaseCurrencyType != model.BaseCurrency))
            {
                throw new CrudOperationException($"Could not edit '{name}' rate, another rate with the same currencies exists");
            }
            rate.SelectedCurrencyType = model.Currency;
            rate.BaseCurrencyType = model.BaseCurrency;
            rate.Name = model.Name;
            await _dbContext.SaveChangesAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("Edited favorite rate, name '{oldName}' --> '{newName}'; selected currency '{oldCur}' --> '{newCur}'; base currency '{oldBase}' --> '{newBase}'",
                name, model.Name, rate.BaseCurrencyType, model.BaseCurrency, rate.BaseCurrencyType, model.BaseCurrency);
            return;
        }
        /// <summary>
        /// delete a favorite rate
        /// </summary>
        /// <param name="name">name of the rate to be deleted</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="CrudOperationException">if something went wrong during a crud operation, specifics included</exception>
        public async Task DeleteFavoriteByNameAsync(string name, CancellationToken cancellationToken)
        {
            var toDelete = await _dbContext.FavoriteExchanges.Where(line => line.Name == name).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (toDelete is null)
            {
                throw new CrudOperationException($"'{name}' rate did not exist");
            }
            _dbContext.FavoriteExchanges.Remove(toDelete);
            await _dbContext.SaveChangesAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("Deleted favorite rate, name '{name}'", name);
        }

        private async Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken)
        {
            var evidence = await _dbContext.FavoriteExchanges.Where(x => x.Name == name).AnyAsync(cancellationToken: cancellationToken);
            if (!evidence)
            {
                return true;
            }
            return false;
        }

        private async Task<bool> IsExchangeSetUniqueAsync(CurrencyType cur, CurrencyType baseCur, CancellationToken cancellationToken)
        {
            var evidence = await _dbContext.FavoriteExchanges.Where(x => x.BaseCurrencyType == baseCur && x.SelectedCurrencyType == cur)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (evidence is null)
            {
                return true;
            }
            return false;
        }
    }
}
