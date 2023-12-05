using DoricoNet.Attributes;
using Lea;

namespace DoricoNet.Responses;

/// <summary>
/// Base class for all Dorico responses.
/// </summary>
[ResponseMessage("responseBase")]
public abstract record DoricoResponseBase : IDoricoResponse, IEvent
{
    /// <inheritdoc/>
    public required virtual string Message { get; init; }

    /// <inheritdoc/>
    public string? RawJson { get; set; }
}
