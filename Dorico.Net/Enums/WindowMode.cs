using System.Text.Json.Serialization;

namespace DoricoNet.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WindowMode
{
    Undefined = 0,
    kSetupMode,
    kWriteMode,
    kEngraveMode,
    kPlayMode,
    kPrintMode
}
