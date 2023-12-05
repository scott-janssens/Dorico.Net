namespace DoricoNet.Responses;

/// <summary>
/// Details on an individual flow
/// </summary>
/// <param name="FlowID">The ID of this flow</param>
/// <param name="FlowName"><The name of this flow/param>
public record Flow(int FlowID, string FlowName)
{
    /// <inheritdoc/>
    public override string ToString() => FlowName;
}
