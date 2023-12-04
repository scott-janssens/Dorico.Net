using DoricoNet.Attributes;

namespace DoricoNet.Responses;

/// <summary>
/// Provides a list of all layouts in a score
/// </summary>
/// <param name="OpenScoreID">The ID of the score to which these layouts belong</param>
/// <param name="Layouts">The list of layouts</param>
[ResponseMessage("layoutslist")]
public record LayoutsListResponse(int OpenScoreID, IEnumerable<Layout> Layouts) : DoricoResponseBase;
