using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Fuse8.BackendInternship.PublicApi.Models.ModelBinders
{
    public class DateBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            var modelValueType = context.Metadata.ModelType;
            if (modelValueType == typeof(DateOnly) || modelValueType == typeof(DateOnly?))
            {
                return new DateOnlyModelBinder();
            }
            return null;
        }
    }
}
