using Fuse8.BackendInternship.PublicApi.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fuse8.BackendInternship.PublicApi.SwaggerFilters
{
    public class HttpCodesDocOpFilter : IOperationFilter
    {
        private static readonly OpenApiMediaType _errorResponseMediaType = new()
        {
            Schema = new()
            {
                Reference = new()
                {
                    Type = ReferenceType.Schema,
                    Id = nameof(ErrorResponse)
                }
            }
        };
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            

            AddResponseCode(StatusCodes.Status404NotFound, "Returns if a currency is not found");
            AddResponseCode(StatusCodes.Status429TooManyRequests, "Returns if ran out of tokens");
            AddResponseCode(StatusCodes.Status500InternalServerError, "Returns if an unknown error happend");

            void AddResponseCode(int responseCode, string description)
            {
                operation.Responses.TryAdd(responseCode.ToString(), new()
                {
                    Description = description,
                    Content = new Dictionary<string, OpenApiMediaType>()
                    {
                        ["application/json"] = _errorResponseMediaType
                    }
                });
            }
        }
    }
}
