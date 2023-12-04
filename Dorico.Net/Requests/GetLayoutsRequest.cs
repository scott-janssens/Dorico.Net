using DoricoNet.Responses;

namespace DoricoNet.Requests;

public record GetLayoutsRequest : DoricoRequestBase<LayoutsListResponse>
{
    public override string Message => "{\"message\": \"getlayouts\"}";

    public override string MessageId => "getlayouts";
}
