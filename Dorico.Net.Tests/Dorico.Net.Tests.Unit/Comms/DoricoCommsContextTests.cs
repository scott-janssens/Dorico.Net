using DoricoNet.Comms;
using DoricoNet.Exceptions;
using DoricoNet.Requests;
using DoricoNet.Responses;
using Lea;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text;

namespace Dorico.Net.Tests.Unit.Comms;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DoricoCommsContextTests
{
    private Mock<IClientWebSocketWrapper> _mockWebSocket;
    private Mock<IEventAggregator> _mockEventAggregator;
    private Mock<ILogger> _mockLogger;
    private DoricoCommsContext _context;
    private GetAppInfoRequest _appInfoRequest;
    private readonly VersionResponse _versionResponse = new("Pro", "5.0.20")
    {
        Message = "version",
        RawJson = "{\r\n\"message\": \"version\",\r\n\"variant\": \"Pro\",\r\n\"number\": \"5.0.20\"\r\n}"
    };

    [SetUp]
    public void SetUp()
    {
        _appInfoRequest = new();

        _mockWebSocket = new Mock<IClientWebSocketWrapper>();
        _mockWebSocket.Setup(ws => ws.State).Returns(WebSocketState.Open);

        _mockEventAggregator = new Mock<IEventAggregator>();

        _mockLogger = new();
        _mockLogger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _context = new DoricoCommsContext(_mockWebSocket.Object, _mockEventAggregator.Object, _mockLogger.Object);
    }

    [Test]
    public async Task ConnectAsync()
    {
        _mockWebSocket.Setup(ws => ws.State).Returns(WebSocketState.None);
        Assert.That(_context.State, Is.EqualTo(WebSocketState.None));

        await _context.ConnectAsync(new ConnectionArguments("1234", "ws://127.0.0.1:1", "1.1", CancellationToken.None));

        _mockWebSocket.Verify(ws => ws.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()), Times.Once());
        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => ((IReadOnlyList<KeyValuePair<string, object?>>)@object)
                .Any(x => x.Value!.ToString() == "Connection opened")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Test]
    public void ConnectAsync_WhenAlreadyConnected_ThrowsException()
    {
        Assert.ThrowsAsync<DoricoConnectedException>(() => _context.ConnectAsync(new ConnectionArguments()));
    }

    [Test]
    public async Task TestStop()
    {
        _mockWebSocket.Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                Task.Delay(100).Wait();
                return new WebSocketReceiveResult(0, WebSocketMessageType.Close, true,
                    WebSocketCloseStatus.NormalClosure, "Closed");
            });

        _context.Start();
        Assert.That(_context.IsRunning, Is.True);

        await _context.StopAsync(CancellationToken.None, -1);

        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => ((IReadOnlyList<KeyValuePair<string, object?>>)@object)
                .Any(x => x.Value!.ToString()!.Contains("Connection closed: status description"))),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Test]
    public void TestStopNotRunning()
    {
        _mockWebSocket.Setup(ws => ws.AssertSocketOpen()).Throws<InvalidOperationException>();
        Assert.ThrowsAsync<InvalidOperationException>(async () => await _context.StopAsync(CancellationToken.None, -1));
    }

    [Test]
    public void SendAsync_WhenNotConnected_ThrowsException()
    {
        _mockWebSocket.Setup(ws => ws.AssertSocketOpen()).Throws<InvalidOperationException>();
        var request = new Mock<IDoricoRequest>();

        Assert.ThrowsAsync<InvalidOperationException>(() => _context.SendAsync(request.Object,
            It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task TestSendAsync()
    {
        var response = await SendAsyncHelper();

        Assert.Multiple(() =>
        {
            Assert.That(_appInfoRequest.Response, Is.Not.Null);
            Assert.That(response, Is.EqualTo(_versionResponse));
            Assert.That(_appInfoRequest.IsAborted, Is.False);

            _mockWebSocket.Verify(ws => ws.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    [Test]
    public async Task TestSendAsyncHideMessage()
    {
        _context.HideMessageTypes.Add(new GetAppInfoRequest().MessageId);
        _context.HideMessageTypes.Add("version");

        await SendAsyncHelper();

        Assert.That(_appInfoRequest.IsAborted, Is.False);
        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Debug),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
    }

    [Test]
    public async Task TestSendAsyncNoEcho()
    {
        _context.Echo = false;
        await SendAsyncHelper();

        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Debug),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
    }

    [Test]
    public async Task TestBadResponse()
    {
        var sequence = new MockSequence();

        _mockWebSocket.InSequence(sequence)
            .Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .Callback((ArraySegment<byte> a, CancellationToken c) =>
            {
                var i = 0;
                Encoding.UTF8.GetBytes("{}").ToList().ForEach(c => a[i++] = c);
            })
            .ReturnsAsync(new WebSocketReceiveResult(2, WebSocketMessageType.Text, true));
        _mockWebSocket.InSequence(sequence)
            .Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WebSocketReceiveResult(0, WebSocketMessageType.Close, true,
                WebSocketCloseStatus.NormalClosure, "Closed"));

        _context.Start();

        while (_context.IsRunning)
        {
            await Task.Delay(100);
        }

        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => ((IReadOnlyList<KeyValuePair<string, object?>>)@object)
                .Any(x => x.Value!.ToString()!.StartsWith("unknown response received"))),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Test]
    public async Task TestStart()
    {
        await Run();

        _mockEventAggregator.Verify(x => x.Publish(It.IsAny<IEvent>()), Times.Never);

        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Debug),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Test]
    public async Task TestHideMessages()
    {
        await Run(dws => dws.HideMessageTypes.Add("version"));

        _mockEventAggregator.Verify(x => x.Publish(It.IsAny<IEvent>()), Times.Never);

        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Debug),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
    }

    [Test]
    public async Task TestNoEcho()
    {
        await Run(dws => dws.Echo = false);

        _mockEventAggregator.Verify(x => x.Publish(It.IsAny<IEvent>()), Times.Never);

        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Debug),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
    }

    [Test]
    public async Task CloseWhileWaitingForResponse()
    {
        _mockWebSocket.Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                Task.Delay(100).Wait();
                return new WebSocketReceiveResult(0, WebSocketMessageType.Close, true,
                    WebSocketCloseStatus.NormalClosure, "Closed");
            });

        _context.Start();
        await _context.SendAsync(_appInfoRequest, It.IsAny<CancellationToken>()).ConfigureAwait(false);
    }

    [Test]
    public async Task PublishUnpromptedResponse()
    {
        const string selectionChangedResponse = "{\"message\":\"selectionchanged\",\"openScoreId\":12}";
        var sequence = new MockSequence();
        _mockWebSocket.InSequence(sequence)
            .Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .Callback((ArraySegment<byte> a, CancellationToken c) =>
            {
                var i = 0;
                Encoding.UTF8.GetBytes(selectionChangedResponse).ToList().ForEach(c => a[i++] = c);
            })
            .ReturnsAsync(new WebSocketReceiveResult(selectionChangedResponse.Length, WebSocketMessageType.Text, true));
        _mockWebSocket.InSequence(sequence)
            .Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WebSocketReceiveResult(0, WebSocketMessageType.Close, true,
                WebSocketCloseStatus.NormalClosure, "Closed"));

        _context.Start();

        while (_context.IsRunning)
        {
            await Task.Delay(100);
        }

        _mockEventAggregator.Verify(x => x.Publish(It.IsAny<IEvent>()), Times.Once);
    }

    [Test]
    public async Task RequestUnpromptedResponse()
    {
        var request = new GetStatusRequest();
        const string statusResponse = "{\"message\":\"status\", \"activeOpenScoreID\": 0 }";
        _mockWebSocket.Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .Callback((ArraySegment<byte> a, CancellationToken c) =>
            {
                var i = 0;
                Encoding.UTF8.GetBytes(statusResponse).ToList().ForEach(c => a[i++] = c);
            })
            .ReturnsAsync(() =>
            {
                Task.Delay(100).Wait();
                return new WebSocketReceiveResult(statusResponse.Length, WebSocketMessageType.Text, true);
            });

        _context.Start();
        var response = await _context.SendAsync(request, It.IsAny<CancellationToken>()).ConfigureAwait(false);

        Assert.That(response, Is.Not.Null);
        _mockEventAggregator.Verify(x => x.Publish(It.IsAny<IEvent>()), Times.Once);
    }

    [Test]
    public async Task PublishUnpromptedResponseAfterAbortedRequestOfSame()
    {
        var request = new GetStatusRequest();
        var response = await _context.SendAsync(request, CancellationToken.None, 10);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Null);
            Assert.That(request.IsAborted, Is.True);
        });

        const string statusResponse = "{\"message\":\"status\", \"activeOpenScoreID\": 0 }";
        var sequence = new MockSequence();
        _mockWebSocket.InSequence(sequence)
            .Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .Callback((ArraySegment<byte> a, CancellationToken c) =>
            {
                var i = 0;
                Encoding.UTF8.GetBytes(statusResponse).ToList().ForEach(c => a[i++] = c);
            })
            .ReturnsAsync(new WebSocketReceiveResult(statusResponse.Length, WebSocketMessageType.Text, true));
        _mockWebSocket.InSequence(sequence)
            .Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WebSocketReceiveResult(0, WebSocketMessageType.Close, true,
                WebSocketCloseStatus.NormalClosure, "Closed"));

        _context.Start();

        while (_context.IsRunning)
        {
            await Task.Delay(100);
        }

        _mockEventAggregator.Verify(x => x.Publish(It.IsAny<IEvent>()), Times.Once);
    }

    [Test]
    public async Task SendAsyncCanceled()
    {
        _mockWebSocket.Setup(ws => ws.State).Returns(WebSocketState.Open);
        _mockWebSocket.Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .Callback((ArraySegment<byte> a, CancellationToken c) =>
            {
                var i = 0;
                Encoding.UTF8.GetBytes(_versionResponse.RawJson!).ToList().ForEach(c => a[i++] = c);
            })
            .ReturnsAsync(new WebSocketReceiveResult(_versionResponse.RawJson!.Length,
                WebSocketMessageType.Text, true));

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(10);

        var result = await _context.SendAsync(_appInfoRequest, cancellationTokenSource.Token);
        _context.Start();

        await Task.Delay(100);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Null);
            Assert.That(_appInfoRequest.Response, Is.Null);
            Assert.That(_appInfoRequest.IsAborted, Is.True);
        });

        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => ((IReadOnlyList<KeyValuePair<string, object?>>)@object)
                .Any(x => x.Value!.ToString()!.Contains("canceled"))),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Test]
    public async Task SendAsyncTimeout()
    {
        _mockWebSocket.Setup(ws => ws.State).Returns(WebSocketState.Open);
        _mockWebSocket.Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .Callback((ArraySegment<byte> a, CancellationToken c) =>
            {
                var i = 0;
                Encoding.UTF8.GetBytes(_versionResponse.RawJson!).ToList().ForEach(c => a[i++] = c);
            })
            .ReturnsAsync(new WebSocketReceiveResult(_versionResponse.RawJson!.Length,
                WebSocketMessageType.Text, true));

        var result = await _context.SendAsync(_appInfoRequest, CancellationToken.None, 10);
        _context.Start();

        await Task.Delay(100);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Null);
            Assert.That(_appInfoRequest.Response, Is.Null);
            Assert.That(_appInfoRequest.IsAborted, Is.True);
        });

        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => ((IReadOnlyList<KeyValuePair<string, object?>>)@object)
                .Any(x => x.Value!.ToString()!.Contains("timed out"))),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Test]
    public async Task SendAsyncErrorResponse()
    {
        var response = new Response("kError", "kNotAllowed")
        {
            Message = "response",
            RawJson = "{\"message\":\"response\",\"code\":\"kError\",\"detail\":\"kNotAllowed\"}"
        };
        _mockWebSocket.Setup(ws => ws.State).Returns(WebSocketState.Open);
        _mockWebSocket.Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .Callback((ArraySegment<byte> a, CancellationToken c) =>
            {
                var i = 0;
                Encoding.UTF8.GetBytes(response.RawJson!).ToList().ForEach(c => a[i++] = c);
            })
            .ReturnsAsync(() =>
            {
                Task.Delay(100).Wait();
                return new WebSocketReceiveResult(response.RawJson!.Length, WebSocketMessageType.Text, true);
            });

        _context.Start();
        var result = (Response?)await _context.SendAsync(_appInfoRequest, CancellationToken.None);

        await Task.Delay(100);

        Assert.Multiple(() =>
        {
            Assert.That(result!.Code, Is.EqualTo("kError"));
            Assert.That(_appInfoRequest.Response, Is.Null);
            Assert.That(_appInfoRequest.IsAborted, Is.False);
            Assert.That(_appInfoRequest.ErrorResponse, Is.Not.Null);
        });

        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => true),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
    }

    [Test]
    public async Task SendAsyncSocketThrows()
    {
        _mockWebSocket.Setup(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<WebSocketMessageType>(),
            It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException());

        var response = await _context.SendAsync(_appInfoRequest, It.IsAny<CancellationToken>()).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Null);
            Assert.That(_appInfoRequest.IsAborted, Is.True);
        });
    }

    private async Task Run(Action<DoricoCommsContext>? setupCallback = null)
    {
        var sequence = new MockSequence();

        _mockWebSocket.InSequence(sequence)
            .Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .Callback((ArraySegment<byte> a, CancellationToken c) =>
            {
                var i = 0;
                Encoding.UTF8.GetBytes(_versionResponse.RawJson!).ToList().ForEach(c => a[i++] = c);
            })
            .ReturnsAsync(new WebSocketReceiveResult(_versionResponse.RawJson!.Length,
                WebSocketMessageType.Text, true));
        _mockWebSocket.InSequence(sequence)
            .Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WebSocketReceiveResult(0, WebSocketMessageType.Text, true));
        _mockWebSocket.InSequence(sequence)
            .Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WebSocketReceiveResult(0, WebSocketMessageType.Close, true,
                WebSocketCloseStatus.NormalClosure, "Closed"));

        setupCallback?.Invoke(_context);

        _context.Start();
        Assert.That(_context.IsRunning, Is.True);

        while (_context.IsRunning)
        {
            await Task.Delay(100);
        }
    }

    private async Task<IDoricoResponse?> SendAsyncHelper(IDoricoResponse? response = null)
    {
        var message = response?.RawJson ?? _versionResponse.RawJson!;
        _mockWebSocket.Setup(x => x.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
            .Callback((ArraySegment<byte> a, CancellationToken c) =>
            {
                var i = 0;
                Encoding.UTF8.GetBytes(message).ToList().ForEach(c => a[i++] = c);
            })
            .ReturnsAsync(() =>
            {
                Task.Delay(100).Wait();
                return new WebSocketReceiveResult(message.Length, WebSocketMessageType.Text, true);
            });

        _context.Start();
        return await _context.SendAsync(_appInfoRequest, It.IsAny<CancellationToken>()).ConfigureAwait(false);
    }

    private class BadEvent : IEvent { }
}
