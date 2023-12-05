using System.Net.WebSockets;

namespace DoricoNet.Comms;

/// <summary>
///  Minimal wrapper for ClientWebSocket class to allow for dependency injection.  Only implements the methods that Dorico.Net uses.
/// </summary>
public interface IClientWebSocketWrapper
{
    /// <summary>
    /// The state of the WebSocket.
    /// </summary>
    WebSocketState State { get; }

    /// <summary>
    /// Opens a connection to a WebSocket at the specified Uri.
    /// </summary>
    /// <param name="uri">A Uri object.</param>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <exception cref="ThrowInvalidOperationException">Thrown when the connection is already open.</exception>
    Task ConnectAsync(Uri uri, CancellationToken cancellationToken);

    /// <summary>
    /// Closes a WebSocket connection.
    /// </summary>
    /// <param name="closeStatus"></param>
    /// <param name="statusDescription"></param>
    /// <exception cref="ThrowInvalidOperationException">Thrown when the WebSocket connection is not open.</exception>
    Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription);

    /// <summary>
    /// Recieve a response from Dorico.
    /// </summary>
    /// <param name="buffer">An ArraySegment to store the response.</param>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <exception cref="ThrowInvalidOperationException">Thrown when the WebSocket connection is not open.</exception>
    /// <returns>A WebSocketReceiveResult object/</returns>
    Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a request to Dorico.
    /// </summary>
    /// <param name="buffer">A ArraySegment storing the request data.</param>
    /// <param name="messageType">A WebSocketMessageType value.</param>
    /// <param name="endOfMessage">True if the buffer contains the end of the message, or false if the request is broken up.</param>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken);
}