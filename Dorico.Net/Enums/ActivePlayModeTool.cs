using System.Text.Json.Serialization;

namespace DoricoNet.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ActivePlayModeTool
{
    Undefined = 0,
    kObjectSelection,
    kErase,
    kDraw,
    kLine,
    kDrumStick
}
