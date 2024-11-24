using DoricoNet.Attributes;
using Lea;
using System.Text.Json.Serialization;

namespace DoricoNet.Responses;

/// <summary>
/// Base class for all Dorico responses.
/// </summary>
[ResponseMessage("responseBase")]
public abstract record DoricoResponseBase : IDoricoResponse, IEvent
{
    /// <inheritdoc/>
    public required virtual string Message { get; init; }

    [JsonIgnore]
    /// <inheritdoc/>
    public string? RawJson { get; set; }
}
