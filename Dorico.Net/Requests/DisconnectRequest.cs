using DoricoNet.Responses;

namespace DoricoNet.Requests;

public record DisconnectRequest : DoricoRequestBase<DisconnectResponse>
{
    public override string Message => "{\"message\": \"disconnect\"}";

    public override string MessageId => "disconnect";
}
