using DoricoNet.Responses;

namespace DoricoNet.Requests;

public record GetCommandsRequest : DoricoRequestBase<CommandListResponse>
{
    public override string Message => "{\"message\": \"getcommands\"}";

    public override string MessageId => "getcommands";
}
