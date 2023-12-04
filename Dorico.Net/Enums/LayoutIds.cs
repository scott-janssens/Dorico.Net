using System.Text.Json.Serialization;

namespace DoricoNet.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LayoutIds
{
    kAll = 0,
    kAllFullScoreLayouts,
    kAllPartLayouts,
    kAllCustomScoreLayouts
}
