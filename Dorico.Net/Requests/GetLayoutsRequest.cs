using DoricoNet.Responses;

namespace DoricoNet.Requests;

/// <summary>
/// Requests information about a the layouts in the active score.
/// </summary>
public record GetLayoutsRequest : DoricoRequestBase<LayoutsListResponse>
{
    /// <inheritdoc/>
    public override string Message => "{\"message\": \"getlayouts\"}";

    /// <inheritdoc/>
    public override string MessageId => "getlayouts";
}
