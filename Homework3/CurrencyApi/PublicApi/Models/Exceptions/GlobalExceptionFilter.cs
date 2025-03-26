using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fuse8.BackendInternship.PublicApi.Models.Exceptions
{
    public class GlobalExceptionFilter: IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {

            if (context.Exception is ApiRequestLimitException LimitException)
            {
                Console.WriteLine($"{nameof(ApiRequestLimitException)} was thrown");
                context.Result = new ObjectResult(new { error = LimitException.Message })
                {
                    StatusCode = (int)HttpStatusCode.TooManyRequests
                };
            }
            else if (context.Exception is CurrencyNotFoundException NotFoundException)
            {
                context.Result = new ObjectResult(new { error = NotFoundException.Message })
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }
            else
            {
                Console.WriteLine("Unknown Exception was thrown");
                context.Result = new ObjectResult(new { error = "Something went wrong" })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

            context.ExceptionHandled = true;
        }
    }
}
