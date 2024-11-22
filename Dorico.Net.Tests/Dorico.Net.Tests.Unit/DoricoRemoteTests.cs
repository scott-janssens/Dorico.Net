using DoricoNet;
using DoricoNet.Comms;
using DoricoNet.Exceptions;
using DoricoNet.Requests;
using DoricoNet.Responses;
using Lea;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;

namespace Dorico.Net.Tests;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DoricoRemoteTests
{
    private Mock<IClientWebSocketWrapper> _mockWebSocket;
    private Mock<IEventAggregator> _mockEventAggregator;
    private Mock<ILogger> _mockLogger;
    private Mock<IDoricoCommsContext> _mockComms;
    private DoricoRemote _remote;

    [SetUp]
    public void SetUp()
    {
        _mockWebSocket = new Mock<IClientWebSocketWrapper>();
        _mockWebSocket.Setup(ws => ws.State).Returns(WebSocketState.Open);

        _mockEventAggregator = new Mock<IEventAggregator>();

        _mockLogger = new();
        _mockLogger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

        _mockComms = new Mock<IDoricoCommsContext>();

        _remote = new(_mockComms.Object, _mockLogger.Object);
    }

    [Test]
    public async Task ConnectAsync()
    {
        var sessionToken = "presetToken";
        var connectionArguments = new ConnectionArguments
        {
            SessionToken = sessionToken,
            Address = "ws://127.0.0.1:1234",
            HandshakeVersion = "1.0",
            CancellationToken = CancellationToken.None
        };

        _mockComms.Setup(x => x.ConnectAsync(It.IsAny<ConnectionArguments>())).Returns(Task.CompletedTask);
        _mockComms.SetupSequence(x => x.State)
            .Returns(WebSocketState.None)
            .Returns(WebSocketState.None)
            .Returns(WebSocketState.Open)
            .Returns(WebSocketState.Open)
            .Returns(WebSocketState.Open);

        _mockComms.Setup(x => x.SendAsync(It.Is<IDoricoRequest>(x => x is ConnectWithSessionRequest), It.IsAny<CancellationToken>(), It.IsAny<int>())).Callback((IDoricoRequest request, CancellationToken token, int timeout) =>
                ((ConnectWithSessionRequest)request).SetResponse(new Response("kConnected", "detail") { Message = "response" }));

        var result = await _remote.ConnectAsync("Test", connectionArguments);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(_remote.SessionToken, Is.EqualTo("presetToken"));
        });

        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => ((IReadOnlyList<KeyValuePair<string, object?>>)@object).Any(x => x.Value!.ToString() == ("Connected to Dorico."))),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Test]
    public void ConnectAsyncNoConnect()
    {
        var sessionToken = "presetToken";
        var connectionArguments = new ConnectionArguments(SessionToken: sessionToken);

        _mockComms.Setup(x => x.ConnectAsync(It.IsAny<ConnectionArguments>())).Returns(Task.CompletedTask);
        _mockComms.SetupSequence(x => x.State)
            .Returns(WebSocketState.None)
            .Returns(WebSocketState.None);

        _mockComms.Setup(x => x.SendAsync(It.Is<IDoricoRequest>(x => x is ConnectWithSessionRequest), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Callback((IDoricoRequest request, CancellationToken token, int timeout) =>
                ((ConnectWithSessionRequest)request).SetResponse(new Response("nope", "detail") { Message = "response" }));

        Assert.ThrowsAsync<DoricoException>(() => _remote.ConnectAsync("Test", connectionArguments));
        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => ((IReadOnlyList<KeyValuePair<string, object?>>)@object).Any(x => x.Value!.ToString() == "Could not connect to Dorico. Make sure Dorico is running.")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => ((IReadOnlyList<KeyValuePair<string, object?>>)@object).Any(x => x.Value!.ToString() == ("Connected to Dorico."))),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
    }

    [Test]
    public void ConnectAsyncTimeout()
    {
        var sessionToken = "presetToken";
        var connectionArguments = new ConnectionArguments(SessionToken: sessionToken);

        _mockComms.Setup(x => x.ConnectAsync(It.IsAny<ConnectionArguments>())).Returns(Task.CompletedTask);
        _mockComms.SetupSequence(x => x.State)
            .Returns(WebSocketState.None)
            .Returns(WebSocketState.Open)
            .Returns(WebSocketState.Open);

        _remote.Timeout = 10;
        Assert.ThrowsAsync<DoricoException>(() => _remote.ConnectAsync("Test", connectionArguments));
        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => ((IReadOnlyList<KeyValuePair<string, object?>>)@object).Any(x => x.Value!.ToString() == ("Connected to Dorico."))),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
    }

    [Test]
    public async Task ConnectAsyncNoToken()
    {
        var connectionArguments = new ConnectionArguments();
        _mockComms.Setup(x => x.ConnectAsync(It.IsAny<ConnectionArguments>())).Returns(Task.CompletedTask);
        _mockComms.SetupSequence(x => x.State)
            .Returns(WebSocketState.None)
            .Returns(WebSocketState.None)
            .Returns(WebSocketState.Open)
            .Returns(WebSocketState.Open)
            .Returns(WebSocketState.Open);

        _mockComms.Setup(x => x.SendAsync(It.Is<IDoricoRequest>(x => x is ConnectRequest), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Callback((IDoricoRequest request, CancellationToken token, int timeout) =>
                ((ConnectRequest)request).SetResponse(new SessionTokenResponse("testToken") { Message = "sessiontoken" }));

        var result = await _remote.ConnectAsync("Test", connectionArguments);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(_remote.SessionToken, Is.EqualTo("testToken"));
        });

        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => ((IReadOnlyList<KeyValuePair<string, object?>>)@object).Any(x => x.Value!.ToString() == ("Connected to Dorico."))),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Test]
    public void ConnectAsyncNoTokenTimeout()
    {
        var connectionArguments = new ConnectionArguments();
        _mockComms.Setup(x => x.ConnectAsync(It.IsAny<ConnectionArguments>())).Returns(Task.CompletedTask);
        _mockComms.SetupSequence(x => x.State)
            .Returns(WebSocketState.None)
            .Returns(WebSocketState.Open)
            .Returns(WebSocketState.Open);

        _remote.Timeout = 10;
        Assert.ThrowsAsync<DoricoException>(() => _remote.ConnectAsync("Test", connectionArguments));
    }

    [Test]
    public async Task DisconnectAsync()
    {
        _mockComms.SetupSequence(x => x.State)
           .Returns(WebSocketState.Open)
           .Returns(WebSocketState.Closed)
           .Returns(WebSocketState.Closed);
        _mockComms.Setup(x => x.SendAsync(It.Is<IDoricoRequest>(x => x is DisconnectRequest), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Callback((IDoricoRequest request, CancellationToken token, int timeout) =>
                ((DisconnectRequest)request).SetResponse(new DisconnectResponse(new WebSocketReceiveResult(1, WebSocketMessageType.Close, true, WebSocketCloseStatus.NormalClosure, "closed."))));

        var result = await _remote.DisconnectAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(_remote.IsConnected, Is.False);
        });

        _mockLogger.Verify(logger => logger.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, @type) => ((IReadOnlyList<KeyValuePair<string, object?>>)@object).Any(x => x.Value!.ToString()!.Contains("Disconnected from Dorico"))),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        _mockLogger.Verify(logger => logger.Log(
           It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
           It.IsAny<EventId>(),
           It.Is<It.IsAnyType>((@object, @type) => ((IReadOnlyList<KeyValuePair<string, object?>>)@object).Any(x => x.Value!.ToString()!.Contains("Connected to Dorico."))),
           It.IsAny<Exception>(),
           It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never);
    }

    [Test]
    public void AssertConnectFail()
    {
        _mockComms.Setup(x => x.State).Returns(WebSocketState.Closed);
        Assert.ThrowsAsync<DoricoNotConnectedException>(() => _remote.GetStatusAsync());
    }

    [Test]
    public void GetCommandNullResponse()
    {
        _mockComms.Setup(x => x.State).Returns(WebSocketState.Open);
        _mockComms.Setup(x => x.SendAsync(It.Is<IDoricoRequest>(x => x is GetCommandsRequest), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync((IDoricoResponse?)null);

        Assert.ThrowsAsync<InvalidOperationException>(async () => await _remote.GetCommandAsync("None"));
    }

    [Test]
    public void GetCommandErrorResponse()
    {
        _mockComms.Setup(x => x.State).Returns(WebSocketState.Open);
        _mockComms.Setup(x => x.SendAsync(It.Is<IDoricoRequest>(x => x is GetCommandsRequest), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(new Response("kError", "Nope") { Message = "response" });

        Assert.ThrowsAsync<DoricoException<Response>>(async () => await _remote.GetCommandAsync("None"));
    }

    [Test]
    public async Task GetEngravingOptionsNullResponse()
    {
        _mockComms.Setup(x => x.State).Returns(WebSocketState.Open);
        _mockComms.Setup(x => x.SendAsync(It.Is<IDoricoRequest>(x => x is GetOptionsRequest), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync((IDoricoResponse?)null);

        var result = await _remote.GetEngravingOptionsAsync();

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetEngravingOptionsErrorResponse()
    {
        _mockComms.Setup(x => x.State).Returns(WebSocketState.Open);
        _mockComms.Setup(x => x.SendAsync(It.Is<IDoricoRequest>(x => x is GetOptionsRequest), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(new Response("kError", "Nope") { Message = "response" });

        Assert.ThrowsAsync<DoricoException<Response>>(async () => await _remote.GetEngravingOptionsAsync());
    }

    [Test]
    public async Task GetLayoutOptionsNullResponse()
    {
        _mockComms.Setup(x => x.State).Returns(WebSocketState.Open);
        _mockComms.Setup(x => x.SendAsync(It.Is<IDoricoRequest>(x => x is GetOptionsRequest), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync((IDoricoResponse?)null);

        var result = await _remote.GetLayoutOptionsAsync(0);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetLayoutOptionsErrorResponse()
    {
        _mockComms.Setup(x => x.State).Returns(WebSocketState.Open);
        _mockComms.Setup(x => x.SendAsync(It.Is<IDoricoRequest>(x => x is GetOptionsRequest), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(new Response("kError", "Nope") { Message = "response" });

        Assert.ThrowsAsync<DoricoException<Response>>(async () => await _remote.GetLayoutOptionsAsync(0));
    }

    [Test]
    public async Task GetNotationOptionsNullResponse()
    {
        _mockComms.Setup(x => x.State).Returns(WebSocketState.Open);
        _mockComms.Setup(x => x.SendAsync(It.Is<IDoricoRequest>(x => x is GetOptionsRequest), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync((IDoricoResponse?)null);

        var result = await _remote.GetNotationOptionsAsync(0);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetNotationOptionsErrorResponse()
    {
        _mockComms.Setup(x => x.State).Returns(WebSocketState.Open);
        _mockComms.Setup(x => x.SendAsync(It.Is<IDoricoRequest>(x => x is GetOptionsRequest), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(new Response("kError", "Nope") { Message = "response" });

        Assert.ThrowsAsync<DoricoException<Response>>(async () => await _remote.GetNotationOptionsAsync(0));
    }
}
