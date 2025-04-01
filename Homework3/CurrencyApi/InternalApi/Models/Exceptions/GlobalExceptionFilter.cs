using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using InternalApi.Models.Exceptions;

namespace InternalApi.Models.Exceptions
{
    public class GlobalExceptionFilter: IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case ApiRequestLimitException LimitException:
                    _logger.LogError(LimitException.Message);
                    SetResponse(LimitException.Message, StatusCodes.Status429TooManyRequests);
                    break;
                case CurrencyNotFoundException NotFoundException:
                    SetResponse(NotFoundException.Message, StatusCodes.Status404NotFound);
                    break;
                default:
                    _logger.LogWarning(context.Exception, "Unknown Exception was thrown");
                    SetResponse("Unknown Exception was thrown", StatusCodes.Status500InternalServerError);
                    break;
            }

            context.ExceptionHandled = true;
            return;
            void SetResponse(string errorMessage, int statusCode)
            {
                context.Result = new JsonResult(new ErrorResponse(errorMessage));
                context.HttpContext.Response.StatusCode = statusCode;   
            }
        }
    }
}
