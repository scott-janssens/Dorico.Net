using DoricoNet.Attributes;

namespace DoricoNet.Responses;

// A base class for responses from Dorico that don't require a request.
[ResponseMessage("unpromptedResponseBase")]
public abstract record DoricoUnpromptedResponseBase : DoricoResponseBase;
