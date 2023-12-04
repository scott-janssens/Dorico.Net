using DoricoNet.Responses;

namespace DoricoNet.Requests;

public record GetStatusRequest : DoricoRequestBase<StatusResponse>
{
    public override string Message => $"{{\"message\": \"getstatus\"}}";

    public override string MessageId => "getstatus";
}
