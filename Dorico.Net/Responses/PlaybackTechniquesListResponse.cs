using DoricoNet.Attributes;

namespace DoricoNet.Responses;

/// <summary>
/// Provides a list of playback techniques defined in expression maps
/// or drum kit note maps associated with a playback endpoint
/// </summary>
/// <param name="OpenScoreID">The ID of the score to which this techniques list belongs</param>
/// <param name="ExpressionMaps">The expression maps associated with the endpoint, if any</param>
/// <param name="DrumKitNoteMaps">DrumKitNoteMap</param>
[ResponseMessage("playbacktechniqueslist")]
public record PlaybackTechniquesListResponse(
    int OpenScoreID,
    IEnumerable<ExpressionMap> ExpressionMaps,
    IEnumerable<DrumKitNoteMap> DrumKitNoteMaps) : DoricoResponseBase;
