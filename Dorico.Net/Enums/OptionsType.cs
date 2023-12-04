using System.Text.Json.Serialization;

namespace DoricoNet.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OptionsType
{
    Undefined = 0,
    kLayout,
    kEngraving,
    kNotation
}
