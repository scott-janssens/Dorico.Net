namespace DoricoNet.Responses;

/// <summary>
/// Details on an individual techniques
/// </summary>
/// <param name="Name">The name of this technique if one is set</param>
/// <param name="InstrumentId">The instrument entity ID associated with this technique (can differ within a
/// percussion kit)</param>
/// <param name="Pitch">The pitch for this technique</param>
/// <param name="KeySwitches">The key switches if any</param>
/// <param name="PlaybackTechniqueIds">The playback technique combination</param>
public record DrumKitNoteMapEntry(
    string? Name,
    string InstrumentId,
    int Pitch,
    IEnumerable<int>? KeySwitches,
    IEnumerable<string>? PlaybackTechniqueIds)
{
    /// <inheritdoc/>
    public override string ToString() => string.IsNullOrWhiteSpace(Name) ? "<DrumKitNoteMapEntry>" : Name;
}
