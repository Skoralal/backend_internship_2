using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fuse8.BackendInternship.PublicApi.Models.ModelBinders
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new SerializationException($"Can not convert string to {nameof(DateOnly)}: value is empty");
            }
            return DateOnly.ParseExact(value, ModelBinderConstants.DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal);
        }
        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(format: ModelBinderConstants.DateFormat, CultureInfo.CurrentCulture));
        }
    }
}
