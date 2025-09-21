using System.Text.Json;
using System.Text.Json.Serialization;

namespace MetaBond.Infrastructure.Shared.JsonConverters;

public class SafeGuidConverter : JsonConverter<Guid?>
{
    public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (string.IsNullOrWhiteSpace(str)) return null;
            if (Guid.TryParse(str, out var guid)) return guid;

            //Exception handling
            throw new JsonException("The categoryId must be a valid GUID.");
        }

        return reader.GetGuid();
    }

    public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString());
    }
}