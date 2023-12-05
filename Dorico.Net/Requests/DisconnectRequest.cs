using DoricoNet.Responses;

namespace DoricoNet.Requests;

/// <summary>
/// Informs Dorico a client is about to close its connection.
/// </summary>
public record DisconnectRequest : DoricoRequestBase<DisconnectResponse>
{
    /// <inheritdoc/>
    public override string Message => "{\"message\": \"disconnect\"}";

    /// <inheritdoc/>
    public override string MessageId => "disconnect";
}
