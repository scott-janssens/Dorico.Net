using DoricoNet.Responses;

namespace DoricoNet.Requests;

public record AcceptSessionTokenRequest(string SessionToken) : DoricoRequestBase<Response>
{
    public override string Message => $"{{ \"message\": \"acceptsessiontoken\", \"sessionToken\": \"{SessionToken}\"}}";

    public override string MessageId => "acceptsessiontoken";
}
