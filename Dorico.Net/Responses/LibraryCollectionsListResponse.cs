using DoricoNet.Attributes;

namespace DoricoNet.Responses;

/// <summary>
/// Provides a list of library collections in a score
/// </summary>
/// <param name="OpenScoreID">The ID of the score to which these layouts belong</param>
/// <param name="Collections">The list of collections</param>
[ResponseMessage("librarycollectionslist")]
public record LibraryCollectionsListResponse(int OpenScoreID, IEnumerable<string> Collections) : DoricoResponseBase;
