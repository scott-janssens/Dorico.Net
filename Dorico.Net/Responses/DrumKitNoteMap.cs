namespace DoricoNet.Responses;

/// <summary>
/// Details on an drum kit note map
/// </summary>
/// <param name="DrumKitNoteMapId">The ID of the drum kit note map</param>
/// <param name="DrumKitNoteMapEntries">The list of techniques in the drum kit note map.Entries only for 
/// the instrument types present in the selection on receipt of the getplaybacktechniques message will be 
/// shown.This could mean the list is empty if no entries apply to these instruments. </param>
public record DrumKitNoteMap(string DrumKitNoteMapId, IEnumerable<DrumKitNoteMapEntry> DrumKitNoteMapEntries);