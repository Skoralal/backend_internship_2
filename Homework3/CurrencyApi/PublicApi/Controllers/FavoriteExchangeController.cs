using Common.Models;
using Fuse8.BackendInternship.PublicApi.Models;
using Fuse8.BackendInternship.PublicApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fuse8.BackendInternship.PublicApi.Controllers
{
    [Route("favorite/")]
    public class FavoriteExchangeController:ControllerBase
    {
        private readonly FavoriteExchangesService _favoriteExchangeService;
        public FavoriteExchangeController(FavoriteExchangesService favoriteExchangeService)
        {
            _favoriteExchangeService = favoriteExchangeService;
        }
        /// <summary>
        /// Add new favorite exchange rate
        /// </summary>
        /// <param name="name">name of the rate</param>
        /// <param name="currency">selected currency</param>
        /// <param name="baseCurrency">base currency</param>
        /// <response code="200">
        /// if successful
        /// </response>
        /// <response code="400">
        /// if database manipulation failed
        /// </response>
        /// <response code="500">
        /// if unexpected error occurred
        /// </response>
        [HttpPost("{name}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> AddFavoriteAsync([FromRoute]string name, [FromHeader] CurrencyType currency, [FromHeader] CurrencyType baseCurrency)
        {
            await _favoriteExchangeService.AddFavoriteAsync(new()
            {
                BaseCurrencyType = baseCurrency,
                SelectedCurrencyType = currency,
                Id = new Guid(),
                Name = name
            });
            return Ok();
        }
        /// <summary>
        /// get an exchange rate with the specified name
        /// </summary>
        /// <param name="name">name of the rate</param>
        /// <response code="200">
        /// if successful
        /// </response>
        /// <response code="400">
        /// if database manipulation failed
        /// </response>
        /// <response code="500">
        /// if unexpected error occurred
        /// </response>
        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<FavoriteRateDBObject>> GetFavoriteByNameAsync([FromRoute] string name)
        {
            var rate = await _favoriteExchangeService.GetFavoriteByNameAsync(name);
            return Ok(rate);
        }
        /// <summary>
        /// get all exchange rates
        /// </summary>
        /// <response code="200">
        /// if successful
        /// </response>
        /// <response code="400">
        /// if database manipulation failed
        /// </response>
        /// <response code="500">
        /// if unexpected error occurred
        /// </response>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<FavoriteRateDBObject[]>> GetAllFavoriteAsync()
        {
            var rates = await _favoriteExchangeService.GetAllFavoriteAsync();
            return Ok(rates);
        }
        /// <summary>
        /// change specified by name rate's parameters to supplied 
        /// </summary>
        /// <param name="name">name of the rate</param>
        /// <param name="newName">new name of the rate(optional)</param>
        /// <param name="currency">selected currency (default to not change)</param>
        /// <param name="baseCurrency">base currency (default to not change)</param>
        /// <response code="200">
        /// if successful
        /// </response>
        /// <response code="400">
        /// if database manipulation failed
        /// </response>
        /// <response code="500">
        /// if unexpected error occurred
        /// </response>
        [HttpPut("{name}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> ReplaceFavoriteByNameAsync([FromRoute] string name, [FromHeader] CurrencyType? currency,
            [FromHeader] CurrencyType? baseCurrency, [FromHeader] string newName = "")
        {
            if(newName == "")
            {
                newName = name;
            }
            if(baseCurrency == CurrencyType.NotSet)
            {
                baseCurrency = null;
            }
            if(currency == CurrencyType.NotSet)
            {
                currency = null;
            }
            await _favoriteExchangeService.ReplaceFavoriteByNameAsync(name, newName, currency, baseCurrency);
            return Ok();
        }
        /// <summary>
        /// delete an exchange rate with the specified name
        /// </summary>
        /// <param name="name">name of the rate</param>
        /// <response code="200">
        /// if successful
        /// </response>
        /// <response code="400">
        /// if database manipulation failed
        /// </response>
        /// <response code="500">
        /// if unexpected error occurred
        /// </response>
        [HttpDelete("{name}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> DeleteFavoriteByNameAsync([FromRoute] string name)
        {
            await _favoriteExchangeService.DeleteFavoriteByNameAsync(name);
            return Ok();
        }
    }
}
