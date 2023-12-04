using DoricoNet.Responses;

namespace DoricoNet.Requests;

public record GetFlowsRequest : DoricoRequestBase<FlowsListResponse>
{
    public override string Message => "{\"message\": \"getflows\"}";

    public override string MessageId => "getflows";
}
