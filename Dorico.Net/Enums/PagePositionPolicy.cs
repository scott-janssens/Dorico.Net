using System.Text.Json.Serialization;

namespace DoricoNet.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PagePositionPolicy
{
    Undefined = 0,
    kSingleHorizontal,
    kSingleVertical,
    kSpreadHorizontal,
    kSpreadVertical,
    kGalleyView
}
