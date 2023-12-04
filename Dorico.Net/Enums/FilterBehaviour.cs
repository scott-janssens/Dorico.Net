using System.Text.Json.Serialization;

namespace DoricoNet.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FilterBehaviour
{
    Undefined = 0,
    kSelect,
    kDeselect
}
