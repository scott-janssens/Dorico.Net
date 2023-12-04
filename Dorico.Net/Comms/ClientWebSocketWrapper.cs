using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;

namespace DoricoNet.Comms;

/// <summary>
///  Minimal wrapper for ClientWebSocket class to allow for dependency injection.  Only implements the methods that Dorico.Net uses.
/// </summary>
[ExcludeFromCodeCoverage]
public partial class ClientWebSocketWrapper : IClientWebSocketWrapper, IDisposable
{
    private bool _disposed;
    private readonly ILogger _logger;
    private ClientWebSocket? _clientWebSocket;

    public WebSocketState State => _clientWebSocket?.State ?? WebSocketState.None;

    #region LoggerMessages

    [LoggerMessage(LogLevel.Information, "Websocket closed by other party: {Description}")]
    partial void LogCloseStatus(string? description);

    #endregion

    public ClientWebSocketWrapper(ILogger logger)
    {
        _logger = logger;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _clientWebSocket?.Dispose();
            }

            _disposed = true;
        }
    }

    public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
    {
        if (State == WebSocketState.Open)
        {
            ThrowHelper.ThrowInvalidOperationException("WebSocket is already connected.");
        }

        _clientWebSocket = new();
        return _clientWebSocket.ConnectAsync(uri, cancellationToken);
    }

    public async Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription)
    {
        if (State != WebSocketState.Open)
        {
            ThrowHelper.ThrowInvalidOperationException("WebSocket is not open.");
        }

        // forcing CancellationToken.None sending an already cancelled token seems to have no effect.
        await _clientWebSocket!.CloseAsync(closeStatus, statusDescription, CancellationToken.None).ConfigureAwait(false);

        _clientWebSocket.Dispose();
        _clientWebSocket = null;
    }

    public Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
    {
        if (State != WebSocketState.Open)
        {
            ThrowHelper.ThrowInvalidOperationException("WebSocket is not open.");
        }

        return _clientWebSocket!.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
    }

    public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
    {
        if (State != WebSocketState.Open)
        {
            ThrowHelper.ThrowInvalidOperationException("WebSocket is not open.");
        }

        var response = await _clientWebSocket!.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);

        if (response.MessageType == WebSocketMessageType.Close)
        {
            LogCloseStatus(response.CloseStatusDescription);

            await _clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None).ConfigureAwait(false);

            _clientWebSocket.Dispose();
            _clientWebSocket = null;
        }

        return response;
    }
}
