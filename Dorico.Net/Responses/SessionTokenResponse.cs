using DoricoNet.Attributes;

namespace DoricoNet.Responses;

[ResponseMessage("sessiontoken")]
public record SessionTokenResponse(string SessionToken) : DoricoResponseBase;
