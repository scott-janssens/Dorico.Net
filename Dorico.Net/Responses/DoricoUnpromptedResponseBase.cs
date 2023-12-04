using DoricoNet.Attributes;

namespace DoricoNet.Responses;

[ResponseMessage("unpromptedResponseBase")]
public abstract record DoricoUnpromptedResponseBase : DoricoResponseBase;
