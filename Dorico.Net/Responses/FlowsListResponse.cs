using DoricoNet.Attributes;

namespace DoricoNet.Responses;

/// <summary>
/// Provides a list of all flows in a score
/// </summary>
/// <param name="OpenScoreID">The ID of the score to which these flows belong</param>
/// <param name="Flows">The list of flows</param>
[ResponseMessage("flowslist")]
public record FlowsListResponse(int OpenScoreID, IEnumerable<Flow> Flows) : DoricoResponseBase;
