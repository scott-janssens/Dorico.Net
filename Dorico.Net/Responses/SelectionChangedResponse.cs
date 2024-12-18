﻿using DoricoNet.Attributes;

namespace DoricoNet.Responses;

/// <summary>
/// Notifies that the selection in a score has changed. The client should re-request 
/// any data they're interested in, such as the current set of properties.
/// </summary>
/// <param name="OpenScoreId">The ID of the score to which the selection belongs</param>
[ResponseMessage("selectionchanged")]
public record SelectionChangedResponse(int OpenScoreId) : DoricoUnpromptedResponseBase;
