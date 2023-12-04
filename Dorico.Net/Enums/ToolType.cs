using System.Text.Json.Serialization;

namespace DoricoNet.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ToolType
{
    Undefined = 0,
    kMarqueeSelect,
    kHand
}
