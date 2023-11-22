using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TobyMeehan.Com.Api;

public class OptionalConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        if (typeToConvert.GetGenericTypeDefinition() != typeof(Optional<>))
        {
            return false;
        }

        return true;
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = typeToConvert.GetGenericArguments()[0];

        return (JsonConverter) Activator.CreateInstance(
            type: typeof(OptionalConverter<>).MakeGenericType(valueType),
            bindingAttr: BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null
            )!;
    }
    
    private class OptionalConverter<T> : JsonConverter<Optional<T>>
    {
        public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = JsonSerializer.Deserialize<T>(ref reader, options);
            return Optional<T>.Of(value!);
        }

        public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
            {
                return;
            }
            
            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}