using System.Text.Json;
using System.Text.Json.Serialization;

namespace DoricoNet.Json;

public class StringNullWhitespaceConverter<T> : JsonConverter<T>
    where T : Enum
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(T);

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var type = typeof(T);
        var value = reader.GetString();

        return string.IsNullOrWhiteSpace(value)
            ? default
            : (T)Enum.Parse(type, value, true);
        // we need to ignore case on the Parse because the capitalization in StatusResponses differs between
        // RhythmicGridResolution and Duration.
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        writer.WriteStringValue(value.ToString());
    }
}
