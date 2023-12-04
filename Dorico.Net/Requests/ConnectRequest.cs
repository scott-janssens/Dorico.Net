using DoricoNet.Responses;

namespace DoricoNet.Requests;

public record ConnectRequest(string ClientName, string HandshakeVersion = "1.0") : DoricoRequestBase<SessionTokenResponse>
{
    public override string Message =>
        $"{{\"message\": \"connect\", \"clientName\": \"{ClientName}\", \"handshakeVersion\": \"{HandshakeVersion}\"}}";

    public override string MessageId => "connect";
}

public record ConnectWithSessionRequest(string ClientName, string SessionToken, string HandshakeVersion = "1.0") : DoricoRequestBase<Response>
{
    public override string Message =>
        $"{{\"message\": \"connect\", \"clientName\": \"{ClientName}\", \"handshakeVersion\": \"{HandshakeVersion}\"{(SessionToken != null ? $",\"sessionToken\":\"{SessionToken}\"" : string.Empty)}}}";

    public override string MessageId => "connect";
}
