using DoricoNet.Responses;

namespace DoricoNet.Requests;

/// <summary>
/// Requests information about the instance of Dorico the client is connected to.
/// </summary>
public record GetAppInfoRequest : DoricoRequestBase<VersionResponse>
{
    /// <inheritdoc/>
    public override string Message => "{\"message\": \"getappinfo\", \"info\": \"version\"}";

    /// <inheritdoc/>
    public override string MessageId => "getappinfo";
}
