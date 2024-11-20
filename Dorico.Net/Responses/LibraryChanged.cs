using DoricoNet.Attributes;

namespace DoricoNet.Responses;

/// <summary>
/// Notifies that changes have taken place in one or more library collections. 
/// The client should re-request any library collection data they're interested in.
/// </summary>
/// <param name="OpenScoreId">The ID of the score to which the changed collections belong</param>
/// <param name="Collections">The list of collections which have changed</param>
[ResponseMessage("librarychanged")]
public record LibraryChanged(int OpenScoreId, IEnumerable<string> Collections) : DoricoUnpromptedResponseBase;
