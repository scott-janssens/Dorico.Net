using DoricoNet.Responses;

namespace DoricoNet.Requests;

public record GetAppInfoRequest : DoricoRequestBase<VersionResponse>
{
    public override string Message => "{\"message\": \"getappinfo\", \"info\": \"version\"}";

    public override string MessageId => "getappinfo";
}
