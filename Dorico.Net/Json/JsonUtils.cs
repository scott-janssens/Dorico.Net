using System.Buffers;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace DoricoNet.Json;

// Taken from https://github.com/dotnet/runtime/issues/31433

public static class JsonUtils
{
    public static string Merge(string originalJson, string newContent, bool camelCase = false)
    {
        var outputBuffer = new ArrayBufferWriter<byte>();

        using (JsonDocument jDoc1 = JsonDocument.Parse(originalJson))
        using (JsonDocument jDoc2 = JsonDocument.Parse(newContent))
        using (var jsonWriter = new Utf8JsonWriter(outputBuffer, new JsonWriterOptions { Indented = true }))
        {
            JsonElement root1 = jDoc1.RootElement;
            JsonElement root2 = jDoc2.RootElement;

            // Assuming both JSON strings are single JSON objects (i.e. {...})
            Debug.Assert(root1.ValueKind == JsonValueKind.Object);
            Debug.Assert(root2.ValueKind == JsonValueKind.Object);

            jsonWriter.WriteStartObject();

            // Write all the properties of the first document that don't conflict with the second
            // Or if the second is overriding it with null, favor the property in the first.
            foreach (JsonProperty property in root1.EnumerateObject())
            {
                var propertyName = camelCase
                    ? JsonNamingPolicy.CamelCase.ConvertName(property.Name)
                    : property.Name;

                if (!root2.TryGetProperty(propertyName, out JsonElement newValue) || newValue.ValueKind == JsonValueKind.Null)
                {
                    property.WriteTo(jsonWriter);
                }
            }

            // Write all the properties of the second document (including those that are duplicates which were skipped earlier)
            // The property values of the second document completely override the values of the first, unless they are null in the second.
            foreach (JsonProperty property in root2.EnumerateObject())
            {
                // Don't write null values, unless they are unique to the second document
                if (property.Value.ValueKind != JsonValueKind.Null || !root1.TryGetProperty(property.Name, out _))
                {
                    property.WriteTo(jsonWriter);
                }
            }

            jsonWriter.WriteEndObject();
        }

        return Encoding.UTF8.GetString(outputBuffer.WrittenSpan);
    }
}