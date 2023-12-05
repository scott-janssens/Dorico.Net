using DoricoNet.Attributes;

namespace DoricoNet.Responses;

/// <summary>
/// A response with a code sent by Dorico to a received message. Only sent in the 
/// case of errors or for commands which don't otherwise have a response of their own.
/// </summary>
/// <param name="Code">The response code</param>
/// <param name="Detail">Optional detail text regarding the returned status</param>
[ResponseMessage("response")]
public record Response(string Code, string? Detail) : DoricoResponseBase
{
    /// <inheritdoc/>
    public override string ToString() => $"Response: {Code} {Detail}";
}
