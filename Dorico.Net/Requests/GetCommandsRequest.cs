using DoricoNet.Responses;

namespace DoricoNet.Requests;

/// <summary>
/// Requests the list of commands and their parameters which Dorico accepts via a command message.
/// </summary>
public record GetCommandsRequest : DoricoRequestBase<CommandListResponse>
{
    /// <inheritdoc/>
    public override string Message => "{\"message\": \"getcommands\"}";

    /// <inheritdoc/>
    public override string MessageId => "getcommands";
}
