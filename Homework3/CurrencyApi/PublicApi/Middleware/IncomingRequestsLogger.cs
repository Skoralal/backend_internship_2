
namespace Fuse8.BackendInternship.PublicApi.Middleware
{
    public class IncomingRequestsLogger:IMiddleware
    {

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Console.WriteLine($"{context.Request.Path} was called at {DateTime.Now}");
            await next(context);
        }
    }
}
