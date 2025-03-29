using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Fuse8.BackendInternship.PublicApi.Models.ModelBinders
{
    public class DateOnlyModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if(valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            var valStr = valueProviderResult.FirstValue;
            if(string.IsNullOrWhiteSpace(valStr))
            {
                DateOnly? defVal = bindingContext.ModelType == typeof(DateOnly?)?null: default(DateOnly);
            }

            var valParsed = DateOnly.ParseExact(valStr, ModelBinderConstants.DateFormat, CultureInfo.CurrentCulture);
            SetValue(valParsed);
            return Task.CompletedTask;


            void SetValue(DateOnly? value)
            {
                bindingContext.Result = ModelBindingResult.Success(value);
            }
        }
    }
}
