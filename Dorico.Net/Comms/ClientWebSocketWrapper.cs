using CommunityToolkit.Diagnostics;
using DoricoNet.Exceptions;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;

namespace DoricoNet.Comms;

/// <summary>
///  Minimal wrapper for ClientWebSocket class to allow for dependency injection.  Only implements the methods that
///  Dorico.Net uses.
/// </summary>
/// <remarks>
/// ClientWebSocketWrapper constructor.
/// </remarks>
/// <param name="logger">A logger object.</param>
[ExcludeFromCodeCoverage]
public partial class ClientWebSocketWrapper(ILogger logger) : IClientWebSocketWrapper, IDisposable
{
    private bool _disposed;
    private ClientWebSocket? _clientWebSocket;

    /// <inheritdoc/>
    public WebSocketState State => _clientWebSocket?.State ?? WebSocketState.None;

    #region LoggerMessages

    [LoggerMessage(LogLevel.Trace, "Unexpected WebSocket status after connection: {Status}")]
    partial void LogNotRunning(string status);

    [LoggerMessage(LogLevel.Trace, "Opening WebSocket: {uri}")]
    partial void LogConnecting(string uri);

    [LoggerMessage(LogLevel.Trace, "WebSocket opened: {uri}")]
    partial void LogConnected(string uri);

    [LoggerMessage(LogLevel.Trace, "Closing WebSocket: [{closeStatus}] {Description}")]
    partial void LogClosing(WebSocketCloseStatus closeStatus, string? description);

    [LoggerMessage(LogLevel.Trace, "WebSocket closed: [{closeStatus}] {Description}")]
    partial void LogClosed(WebSocketCloseStatus closeStatus, string? description);

    [LoggerMessage(LogLevel.Information, "WebSocket closed by other party: {Description}")]
    partial void LogCloseStatus(string? description);

    [LoggerMessage(LogLevel.Trace, "Error sending data to WebSocket: {Message}")]
    partial void LogSendError(string message);

    [LoggerMessage(LogLevel.Trace, "Sent data to WebSocket, Type: {MessageType}, EoM: {EndOfMessage}")]
    partial void LogSent(WebSocketMessageType messageType, bool endOfMessage);

    [LoggerMessage(LogLevel.Trace, "Received data from WebSocket: {Data}")]
    partial void LogReceived(string? data);

    #endregion

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(uri);

        if (State == WebSocketState.Open)
        {
            throw new InvalidOperationException("WebSocket is already connected.");
        }

        LogConnecting(uri.ToString());

        _clientWebSocket = new();
        var result = _clientWebSocket.ConnectAsync(uri, cancellationToken);

        if (result.Status != TaskStatus.WaitingForActivation &&
            result.Status != TaskStatus.Running)
        {
            LogNotRunning(result.Status.ToString());
            throw new DoricoException($"Unexpected WebSocket status after connection: {result.Status}");
        }

        LogConnected(uri.ToString());

        return result;
    }

    /// <inheritdoc/>
    public async Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription)
    {
        AssertSocketOpen();

        LogClosing(closeStatus, statusDescription);

        // forcing CancellationToken.None sending an already canceled token seems to have no effect.
        await _clientWebSocket!.CloseAsync(closeStatus, statusDescription, CancellationToken.None)
            .ConfigureAwait(false);

        LogClosed(closeStatus, statusDescription);

        _clientWebSocket.Dispose();
        _clientWebSocket = null;
    }

    /// <inheritdoc/>
    public async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage,
        CancellationToken cancellationToken)
    {
        AssertSocketOpen();

        try
        {
            await _clientWebSocket!.SendAsync(buffer, messageType, endOfMessage, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            LogSendError(ex.Message);
            throw;
        }
        LogSent(messageType, endOfMessage);
    }

    /// <inheritdoc/>
    public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer,
        CancellationToken cancellationToken)
    {
        AssertSocketOpen();

        var response = await _clientWebSocket!.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
        LogReceived(buffer.ToString());

        if (response.MessageType == WebSocketMessageType.Close)
        {
            LogCloseStatus(response.CloseStatusDescription);

            if (_clientWebSocket != null)
            {
                await _clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None).ConfigureAwait(false);

                _clientWebSocket.Dispose();
                _clientWebSocket = null;
            }
        }

        return response;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AssertSocketOpen()
    {
        if (State != WebSocketState.Open)
        {
            throw new InvalidOperationException($"WebSocket connection is not open: {State}.");
        }
    }
}
