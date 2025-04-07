using InternalApi.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace InternalApi.SwaggerFilters
{
    /// <summary>
    /// Adds swagger description for the error response type
    /// </summary>
    public class ErrorResponseDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            context.SchemaGenerator.GenerateSchema(typeof(ErrorResponse), context.SchemaRepository);
        }
    }
}
