using System.Net.WebSockets;

namespace DoricoNet.Comms;

/// <summary>
///  Minimal wrapper for ClientWebSocket class to allow for dependency injection.  Only implements the methods that Dorico.Net uses.
/// </summary>
public interface IClientWebSocketWrapper
{
    WebSocketState State { get; }

    Task ConnectAsync(Uri uri, CancellationToken cancellationToken);

    Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription);

    Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken);

    Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken);
}