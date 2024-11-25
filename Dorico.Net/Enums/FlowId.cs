using System.Text.Json.Serialization;

namespace DoricoNet.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FlowId
{
    kAll = 0,
}
