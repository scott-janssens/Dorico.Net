namespace DoricoNet.Responses;

/// <summary>
/// Details on an individual technique
/// </summary>
/// <param name="Type">The type of technique. Permitted values: kBase, kAddOn</param>
/// <param name="PlaybackTechniqueIds">The playback technique combination</param>
public record ExpressionMapEntry(string Type, IEnumerable<string> PlaybackTechniqueIds)
{
    public override string ToString() => Type;
}
