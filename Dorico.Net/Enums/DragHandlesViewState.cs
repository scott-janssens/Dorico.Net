using System.Text.Json.Serialization;

namespace DoricoNet.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DragHandlesViewState
{
    Undefined = 0,
    kSelected,
    kAll
}
