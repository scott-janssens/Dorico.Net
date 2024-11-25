using DoricoNet.Attributes;

namespace DoricoNet.Responses;

/// <summary>
/// Notifies that options have been changed. The client should re-request 
/// any options they're interested in to get the up to date values.
/// </summary>
/// <param name="OpenScoreId">The ID of the score to which the changed options belong</param>
[ResponseMessage("optionschanged")]
public record OptionsChangedResponse(int OpenScoreId) : DoricoUnpromptedResponseBase;
