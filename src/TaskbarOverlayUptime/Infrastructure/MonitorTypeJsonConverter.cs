using System.Text.Json;
using System.Text.Json.Serialization;
using TaskbarOverlayUptime.Models;

namespace TaskbarOverlayUptime.Infrastructure;

public sealed class MonitorTypeJsonConverter : JsonConverter<MonitorType>
{
    public override MonitorType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Monitor type must be a string.");
        }

        var raw = reader.GetString();
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new JsonException("Monitor type cannot be empty.");
        }

        if (Enum.TryParse<MonitorType>(raw, ignoreCase: true, out var value))
        {
            return value;
        }

        throw new JsonException($"Unsupported monitor type: {raw}");
    }

    public override void Write(Utf8JsonWriter writer, MonitorType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
