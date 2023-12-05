using DoricoNet.Responses;

namespace DoricoNet.Requests;

/// <summary>
/// Requests a list of the techniques defined in the expression map of the playback endpoint of the top most
/// instrument in the current selection.
/// </summary>
public record GetPlaybackTechniquesRequest : DoricoRequestBase<PlaybackTechniquesListResponse>
{
    /// <inheritdoc/>
    public override string Message => "{ \"message\": \"getplaybacktechniques\"}";

    /// <inheritdoc/>
    public override string MessageId => "getplaybacktechniques";
}
