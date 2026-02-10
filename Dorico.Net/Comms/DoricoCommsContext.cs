using CommunityToolkit.Diagnostics;
using DoricoNet.Attributes;
using DoricoNet.Exceptions;
using DoricoNet.Json;
using DoricoNet.Requests;
using DoricoNet.Responses;
using Lea;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DoricoNet.Comms;

/// <summary>
/// Abstraction for communicating with Dorico.
/// 
/// Dorico communicates via a WebSocket, when Dorico.Net sends a request, Dorico sends a response. These messages are
/// in JSON format.
/// 
/// Dorico's responses return in the same order as it received the requests. If multiple requests are sent in quick
/// succession, the responses will be in the same order.  When calling SendAsync(), the method waits for its response
/// before returning.
/// 
/// Dorico also sends some messages without being tromped by a request.  StatusResponse and SelectionChanged are good
/// examples. When this happens, The responses are published via an event aggregator (Lea). 
/// </summary>
/// <remarks>
/// Constructor
/// </remarks>
/// <param name="doricoWebSocket">Dorico specific WebSocket object</param>
/// <param name="eventAggregator">Event aggregator instance</param>
public partial class DoricoCommsContext(
    IClientWebSocketWrapper socket,
    IEventAggregator eventAggregator,
    ILogger logger) : IDoricoCommsContext
{
    private static readonly object statusLock = new();
    private readonly ConcurrentQueue<RequestInfo> _responseQueue = new();
    private readonly Dictionary<string, Type> _responseTypeMap = BuildResponseTypeMap();
	private readonly JsonSerializerOptions _jsonSerializationOptions = CreateJsonOptions();
	private static JsonSerializerOptions CreateJsonOptions()
	{
		var o = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			NumberHandling = JsonNumberHandling.AllowReadingFromString
		};

		// This makes ALL enums tolerant (WindowMode, ToolType, NoteInputMode, etc.)
		o.Converters.Add(new SafeEnumJsonConverterFactory());

		return o;
	}
	private string _lastStatusContent = JsonSerializer.Serialize(StatusResponse.Create());

    #region LoggerMessages

    [LoggerMessage(LogLevel.Information, "Connection opened")]
    partial void LogConnectionOpened();

    [LoggerMessage(LogLevel.Information, "Connection closed: status description: {CloseStatusDescription}")]
    partial void LogConnectionClosed(string? closeStatusDescription);

    [LoggerMessage(LogLevel.Debug, "Message received: {Message}\n{Content}")]
    partial void LogMessageReceived(string message, string content);

    [LoggerMessage(LogLevel.Debug, "sent Request: {RequestType}\n{Content}")]
    partial void LogMessageSent(Type requestType, string content);

    [LoggerMessage(LogLevel.Error, "unknown response received:\n{Content}")]
    partial void LogUnknownResponseError(string content);

    [LoggerMessage(LogLevel.Information, "Request '{RequestType}' canceled.")]
    partial void LogRequestCanceled(string requestType);

    [LoggerMessage(LogLevel.Information, "Request '{RequestType}' timed out.")]
    partial void LogRequestTimedOut(string requestType);

    #endregion

    /// <inheritdoc/>
    public WebSocketState State => socket.State;

    /// <inheritdoc/>
    public Collection<string> HideMessageTypes { get; } = [];

    /// <inheritdoc/>
    public bool Echo { get; set; } = true;

    /// <inheritdoc/>
    public bool IsRunning { get; private set; }

    private StatusResponse? _currentStatus;
    /// <inheritdoc/>
    public StatusResponse? CurrentStatus
    {
        get
        {
            lock (statusLock)
            {
                return _currentStatus;
            }
        }
        private set
        {
            lock (statusLock)
            {
                _currentStatus = value;
            }
        }
    }

    /// <inheritdoc/>
    public async Task ConnectAsync(IConnectionArguments connectionArgs)
    {
        Guard.IsNotNull(connectionArgs, nameof(connectionArgs));

        if (State == WebSocketState.Open)
        {
            throw new DoricoConnectedException();
        }

        _responseQueue.Clear();

        await socket.ConnectAsync(new Uri(connectionArgs.Address),
            connectionArgs.CancellationToken ?? CancellationToken.None).ConfigureAwait(false);
        LogConnectionOpened();

        Start();
    }

    /// <inheritdoc/>
    public void Start()
    {
        if (State == WebSocketState.Open && !IsRunning)
        {
            IsRunning = true;

            _ = Task.Factory.StartNew(async () =>
            {
                var responseBytes = new byte[1024];
                var responseBuffer = new ArraySegment<byte>(responseBytes);

                while (IsRunning)
                {
                    var responseInfo = await GetNextResponse(responseBuffer).ConfigureAwait(false);
                    var isDisconnected = await IsDisconnected(responseInfo.Response).ConfigureAwait(false);

                    if (!isDisconnected && responseInfo.Content.Length > 0)
                    {
                        HandleWebsocketResponse(responseInfo.Content);
                    }
                }
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }

    private async Task<bool> IsDisconnected(WebSocketReceiveResult response)
    {
        var result = false;

        if (response.MessageType == WebSocketMessageType.Close)
        {
            LogConnectionClosed(response.CloseStatusDescription);
            IsRunning = false;

            // clear out request queue
            while (_responseQueue.TryDequeue(out var item))
            {
                if (!item.IsAborted)
                {
                    item.ResetEvent.Set();
                }
            }

            // Close response has no body so can't be processed by HandleWebsocketResponse.
            await eventAggregator.PublishAsync(new DisconnectResponse(response)).ConfigureAwait(false);

            result = true;
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task StopAsync(CancellationToken cancellationToken, int timeout = -1)
    {
        socket.AssertSocketOpen();

        var request = new DisconnectRequest();
        await SendAsync(request, cancellationToken, timeout).ConfigureAwait(false);

        if (request.IsAborted)
        {
            throw new DoricoException("Request was canceled or timed out.");
        }
    }

    /// <inheritdoc/>
    public async Task<IDoricoResponse?> SendAsync(
        IDoricoRequest request,
        CancellationToken cancellationToken,
        int timeout = 30000)
    {
        Guard.IsNotNull(request, nameof(request));
        socket.AssertSocketOpen();

        var sendBytes = Encoding.UTF8.GetBytes(request.Message);
        var sendBuffer = new ArraySegment<byte>(sendBytes);
        var resetEvent = new ManualResetEvent(false);
        var requestInfo = new RequestInfo(request, resetEvent);

        _responseQueue.Enqueue(requestInfo);

#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            await socket.SendAsync(sendBuffer, WebSocketMessageType.Text, true, cancellationToken)
                .ConfigureAwait(false);

            if (Echo && !HideMessageTypes.Contains(request.MessageId))
            {
                LogMessageSent(request.GetType(), request.Message);
            }

            var waitResult = WaitHandle.WaitAny([resetEvent, cancellationToken.WaitHandle], timeout);

            switch (waitResult)
            {
                case WaitHandle.WaitTimeout:
                    requestInfo.IsTimedOut = true;
                    ((DoricoRequestBase)request).Abort();
                    LogRequestTimedOut(requestInfo.GetType().Name);
                    break;
                case 1:
                    requestInfo.IsCanceled = true;
                    ((DoricoRequestBase)request).Abort();
                    LogRequestCanceled(requestInfo.GetType().Name);
                    break;
                default:
                    break;
            }
        }
        catch (TaskCanceledException)
        {
            requestInfo.IsCanceled = true;
            ((DoricoRequestBase)request).Abort();
            LogRequestCanceled(requestInfo.GetType().Name);
        }
        catch
        {
            ((DoricoRequestBase)request).Abort();
        }
        finally
        {
            resetEvent.Dispose();
        }
#pragma warning restore CA1031 // Do not catch general exception types

        return request.Response ?? request.ErrorResponse;
    }

    private void ResponseHandler(IEvent evt)
    {
        var response = (DoricoResponseBase)evt;

        if (_responseQueue.TryPeek(out var requestInfo))
        {
            if (requestInfo.IsAborted)
            {
                _responseQueue.TryDequeue(out _);
                ResponseHandler(evt);
                return;
            }
            else if (response is Response errorResponse && errorResponse.Code == "kError")
            {
                _responseQueue.TryDequeue(out _);
                ((DoricoRequestBase)requestInfo.Request).SetErrorResponse(errorResponse);
                requestInfo.ResetEvent.Set();
            }
            else if (requestInfo.Request.ResponseType == response.GetType())
            {
                _responseQueue.TryDequeue(out _);
                ((DoricoRequestBase)requestInfo.Request).SetResponse(response);
                requestInfo.ResetEvent.Set();
            }
        }

        if (response is DoricoUnpromptedResponseBase)
        {
            if (response is StatusResponse status)
            {
                CurrentStatus = status;
            }

            eventAggregator.Publish(evt);
        }
    }

    private async Task<(WebSocketReceiveResult Response, string Content)> GetNextResponse(
        ArraySegment<byte> responseBuffer)
    {
        WebSocketReceiveResult response;
        StringBuilder sb = new();

        do
        {
            response = await socket.ReceiveAsync(responseBuffer, CancellationToken.None).ConfigureAwait(false);

            if (response.Count == 0)
            {
                continue;
            }

            var messageBytes = responseBuffer.Skip(responseBuffer.Offset).Take(response.Count).ToArray();
            sb.Append(Encoding.UTF8.GetString(messageBytes));
        }
        while (!response.EndOfMessage);

        return (response, sb.ToString());
    }

    private void HandleWebsocketResponse(string content)
    {
        var jObj = JsonNode.Parse(content);
        var message = jObj!["message"]?.ToString();

        if (message == null || !_responseTypeMap.TryGetValue(message, out var responseType))
        {
            LogUnknownResponseError(content);
            return;
        }

        DoricoResponseBase? responseObj = null;

        if (content.Contains("\"message\": \"status\"", StringComparison.Ordinal))
        {
            if (!_responseQueue.TryPeek(out var requestInfo) || 
                requestInfo!.Request.ResponseType != typeof(StatusResponse))
            {
                // non requested status response are likely to be partial payloads.
                // Apply these as a patch to the current status.

                _lastStatusContent = JsonUtils.Merge(_lastStatusContent, content, true);
                try
                {               
                    responseObj = JsonSerializer.Deserialize<StatusResponse>(_lastStatusContent,
                        _jsonSerializationOptions);
                }
                catch 
                {
					// Handle deserialization errors
					LogUnknownResponseError(content);
				}
            }
            else
            {
                _lastStatusContent = content;
            }
        }

        try
        {        
            responseObj ??= (DoricoResponseBase?)JsonSerializer.Deserialize(
            content,
            responseType,
            _jsonSerializationOptions);
		}
		catch
		{
			// Handle deserialization errors
			LogUnknownResponseError(content);
		}

		if (responseObj != null)
        {
            responseObj.RawJson = content;

            if (Echo && !HideMessageTypes.Contains(message!))
            {
                LogMessageReceived(message, content);
            }

            ResponseHandler(responseObj);
        }
    }

    private static Dictionary<string, Type> BuildResponseTypeMap()
    {
        var map = new Dictionary<string, Type>();
        var responseTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => typeof(DoricoResponseBase).IsAssignableFrom(x));

        foreach (var type in responseTypes)
        {
            string messageId = ((ResponseMessageAttribute)type
                .GetCustomAttributes(typeof(ResponseMessageAttribute), true).Single()).MessageId;

            if (messageId != null)
            {
                map[messageId] = type;
            }
        }

        return map;
    }

    private record RequestInfo(IDoricoRequest Request, ManualResetEvent ResetEvent)
    {
        public bool IsCanceled { get; set; }

        public bool IsTimedOut { get; set; }

        public bool IsAborted => IsCanceled || IsTimedOut;
    }
}
