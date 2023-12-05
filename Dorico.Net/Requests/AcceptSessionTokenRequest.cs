using DoricoNet.Responses;

namespace DoricoNet.Requests;

/// <summary>
/// Tells Dorico that the session token has been accepted
/// </summary>
/// <param name="SessionToken">The session token which was accepted. Must match the token which was sent to the client.</param>
public record AcceptSessionTokenRequest(string SessionToken) : DoricoRequestBase<Response>
{
    /// <inheritdoc/>
    public override string Message => $"{{ \"message\": \"acceptsessiontoken\", \"sessionToken\": \"{SessionToken}\"}}";

    /// <inheritdoc/>
    public override string MessageId => "acceptsessiontoken";
}
