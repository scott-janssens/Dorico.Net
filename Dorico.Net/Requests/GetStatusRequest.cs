using DoricoNet.Responses;

namespace DoricoNet.Requests;

/// <summary>
/// Requests Dorico echos back the current status.
/// </summary>
public record GetStatusRequest : DoricoRequestBase<StatusResponse>
{
    /// <inheritdoc/>
    public override string Message => $"{{\"message\": \"getstatus\"}}";

    /// <inheritdoc/>
    public override string MessageId => "getstatus";
}
