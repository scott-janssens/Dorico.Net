using System.Text.Json.Serialization;

namespace DoricoNet.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LayoutType
{
    kAll = 0,
    kFullScoreLayout,
    kPartLayout,
    kCustomScoreLayout
}
