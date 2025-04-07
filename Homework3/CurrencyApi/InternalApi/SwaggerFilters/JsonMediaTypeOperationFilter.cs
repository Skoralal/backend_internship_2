using System.Net.Mime;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace InternalApi.SwaggerFilters
{
    public class JsonMediaTypeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var key in operation.Responses.Keys)
            {
                var openApiMediaTypes = operation.Responses[key].Content;
                if (openApiMediaTypes.TryGetValue(MediaTypeNames.Application.Json, out var appJson))
                {
                    openApiMediaTypes.Clear();
                    openApiMediaTypes.Add(MediaTypeNames.Application.Json, appJson);
                }
            }
        }
    }
}
