using DoricoNet.Responses;

namespace DoricoNet.Requests;

/// <summary>
/// Once a connection to the websocket is established, it is required to send a Connect message in order to 
/// allow communication with Dorico. If Dorico or the user blocks the connection attempt then the websocket will be dropped.
/// </summary>
/// <param name="ClientName">The name to present to the user when asking whether communication should be allowed.</param>
/// <param name="HandshakeVersion">The version of the handshaking protocol the client wants to use</param>
public record ConnectRequest(string ClientName, string HandshakeVersion = "1.0") : DoricoRequestBase<SessionTokenResponse>
{
    /// <inheritdoc/>
    public override string Message =>
        $"{{\"message\": \"connect\", \"clientName\": \"{ClientName}\", \"handshakeVersion\": \"{HandshakeVersion}\"}}";

    /// <inheritdoc/>
    public override string MessageId => "connect";
}

/// <summary>
/// Once a connection to the websocket is established, it is required to send a Connect message in order to 
/// allow communication with Dorico. If Dorico or the user blocks the connection attempt then the websocket will be dropped.
/// </summary>
/// <param name="ClientName">The name to present to the user when asking whether communication should be allowed.</param>
/// <param name="SessionToken">Session token previously provided by Dorico. If valid Dorico will accept this connection 
/// without prompting the user.</param>
/// <param name="HandshakeVersion">The version of the handshaking protocol the client wants to use</param>
public record ConnectWithSessionRequest(string ClientName, string SessionToken, string HandshakeVersion = "1.0") : DoricoRequestBase<Response>
{
    /// <inheritdoc/>
    public override string Message =>
        $"{{\"message\": \"connect\", \"clientName\": \"{ClientName}\", \"handshakeVersion\": \"{HandshakeVersion}\"{(SessionToken != null ? $",\"sessionToken\":\"{SessionToken}\"" : string.Empty)}}}";

    /// <inheritdoc/>
    public override string MessageId => "connect";
}
