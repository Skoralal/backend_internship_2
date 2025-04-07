using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace InternalApi.Services
{
    public class IncomingRequestsLogger:IMiddleware
    {
        private readonly ILogger<IncomingRequestsLogger> _logger;
        public IncomingRequestsLogger(ILogger<IncomingRequestsLogger> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var request = context.Request;
            _logger.LogInformation("Incoming request{Method} {URL}", request.Method, request.Path);
            await next(context);
            var responce = context.Response;
            if (responce.StatusCode == 200) 
            { 
                _logger.LogInformation("Request {Method} {URL} comleted with code {StatusCode}", request.Method, request.Path, responce.StatusCode);
            }
            else
            {
                _logger.LogWarning("Request {Method} {URL} comleted with code {StatusCode}", request.Method, request.Path, responce.StatusCode);
            }
        }
    }
}
