using Fuse8.BackendInternship.PublicApi.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fuse8.BackendInternship.PublicApi.SwaggerFilters
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
