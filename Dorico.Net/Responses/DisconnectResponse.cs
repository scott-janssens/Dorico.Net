using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;

namespace DoricoNet.Responses;

public record DisconnectResponse(WebSocketCloseStatus? CloseStatus, string? CloseStatusDescription) : DoricoResponseBase
{
    [SetsRequiredMembers]
    public DisconnectResponse(WebSocketReceiveResult socketResult)
        : this(socketResult?.CloseStatus, socketResult?.CloseStatusDescription)
    {
        Message = "disconnect";
    }
}
