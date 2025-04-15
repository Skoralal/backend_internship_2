using Common.Models.Exceptions;
using Fuse8.BackendInternship.PublicApi.Models;
using Fuse8.BackendInternship.PublicApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fuse8.BackendInternship.PublicApi.Controllers
{
    [Route("favorite/")]
    public class FavoriteExchangeController : ControllerBase
    {
        private readonly FavoriteExchangesService _favoriteExchangeService;
        public FavoriteExchangeController(FavoriteExchangesService favoriteExchangeService)
        {
            _favoriteExchangeService = favoriteExchangeService;
        }
        /// <summary>
        /// Add new favorite exchange rate
        /// </summary>
        /// <param name="model">model to be sent</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">
        /// if successful
        /// </response>
        /// <response code="400">
        /// if database manipulation failed
        /// </response>
        /// <response code="500">
        /// if unexpected error occurred
        /// </response>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> AddFavoriteAsync([FromBody] FavoriteRateRequestModel model, CancellationToken cancellationToken)
        {
            if (!Enum.IsDefined(model.Currency) || !Enum.IsDefined(model.BaseCurrency))
            {
                throw new CurrencyNotFoundException();
            }
            await _favoriteExchangeService.AddFavoriteAsync(new()
            {
                BaseCurrencyType = model.BaseCurrency,
                SelectedCurrencyType = model.Currency,
                Name = model.Name
            }, cancellationToken: cancellationToken);
            return Ok();
        }
        /// <summary>
        /// get an exchange rate with the specified name
        /// </summary>
        /// <param name="name">name of the rate</param>
        /// <param name="cancellationToken"></param>
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
        public async Task<ActionResult<FavoriteRateDB>> GetFavoriteByNameAsync([FromRoute] string name, CancellationToken cancellationToken)
        {
            var rate = await _favoriteExchangeService.GetFavoriteByNameAsync(name, cancellationToken: cancellationToken);
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
        public async Task<ActionResult<FavoriteRateDB[]>> GetAllFavoriteAsync(CancellationToken cancellationToken)
        {
            var rates = await _favoriteExchangeService.GetAllFavoriteAsync(cancellationToken: cancellationToken);
            return Ok(rates);
        }
        /// <summary>
        /// change specified by name rate's parameters to supplied 
        /// </summary>
        /// <param name="name">name of the rate</param>
        /// <param name="model">model to be sent</param>
        /// <param name="cancellationToken"></param>
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
        public async Task<ActionResult> ReplaceFavoriteByNameAsync([FromRoute] string name, [FromBody] FavoriteRateRequestModel model,
            CancellationToken cancellationToken)
        {
            if (!Enum.IsDefined(model.Currency) || !Enum.IsDefined(model.BaseCurrency))
            {
                throw new CurrencyNotFoundException();
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                model.Name = name;
            }
            await _favoriteExchangeService.ReplaceFavoriteByNameAsync(name, model, cancellationToken: cancellationToken);
            return Ok();
        }
        /// <summary>
        /// delete an exchange rate with the specified name
        /// </summary>
        /// <param name="name">name of the rate</param>
        /// <param name="cancellationToken"></param>
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
        public async Task<ActionResult> DeleteFavoriteByNameAsync([FromRoute] string name, CancellationToken cancellationToken)
        {
            await _favoriteExchangeService.DeleteFavoriteByNameAsync(name, cancellationToken: cancellationToken);
            return Ok();
        }
    }
}
