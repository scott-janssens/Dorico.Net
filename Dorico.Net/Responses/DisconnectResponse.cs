using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;

namespace DoricoNet.Responses;

/// <summary>
/// Details on disconnecting with Dorico.
/// </summary>
/// <param name="CloseStatus">A WebSocketCloseStatus value.</param>
/// <param name="CloseStatusDescription">A description of why the connection was closed or null.</param>
public record DisconnectResponse(WebSocketCloseStatus? CloseStatus, string? CloseStatusDescription) : DoricoResponseBase
{
    [SetsRequiredMembers]
    public DisconnectResponse(WebSocketReceiveResult socketResult)
        : this(socketResult?.CloseStatus, socketResult?.CloseStatusDescription)
    {
        Message = "disconnect";
    }
}
