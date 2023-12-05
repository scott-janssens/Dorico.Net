using DoricoNet.Responses;

namespace DoricoNet.Requests;

/// <summary>
/// Requests information about the flows in the active score.
/// </summary>
public record GetFlowsRequest : DoricoRequestBase<FlowsListResponse>
{
    /// <inheritdoc/>
    public override string Message => "{\"message\": \"getflows\"}";

    /// <inheritdoc/>
    public override string MessageId => "getflows";
}
