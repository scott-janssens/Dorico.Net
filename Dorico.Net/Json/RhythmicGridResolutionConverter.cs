using DoricoNet.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DoricoNet.Json;

public class RhythmicGridResolutionConverter : JsonConverter<RhythmicGridResolution>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(RhythmicGridResolution);
    }

    public override RhythmicGridResolution Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        return string.IsNullOrWhiteSpace(value)
            ? RhythmicGridResolution.Undefined
            : (RhythmicGridResolution)Enum.Parse(typeof(RhythmicGridResolution), value);
    }

    public override void Write(Utf8JsonWriter writer, RhythmicGridResolution value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));

        writer.WriteStringValue(value.ToString());
    }
}
