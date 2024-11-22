using DoricoNet;
using DoricoNet.Commands;
using DoricoNet.Comms;
using DoricoNet.Enums;
using DoricoNet.Exceptions;
using DoricoNet.Requests;
using DoricoNet.Responses;
using Lea;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Dorico.Net.Tests.Integration_Tests;

[ExcludeFromCodeCoverage]
[TestFixture]
public class DoricoRemoteIntegrationTests
{
    private const string DoricoFileName = "Dorico5";
    private const string _clientName = "Dorico.Net Integration Test";
    private readonly ServiceProvider _serviceProvider;
    private string? _sessionToken;
    private IDoricoRemote _remote;

    public DoricoRemoteIntegrationTests()
    {
        var services = new ServiceCollection()
            .AddSingleton(sp => LoggerFactory.Create(builder => builder.AddDebug().SetMinimumLevel(LogLevel.Trace)).CreateLogger("Dorico.Net"))
            .AddSingleton<IEventAggregator, EventAggregator>()
            .AddTransient<IClientWebSocketWrapper, ClientWebSocketWrapper>()
            .AddSingleton<IDoricoCommsContext, DoricoCommsContext>()
            .AddTransient<IDoricoRemote, DoricoRemote>();

        _serviceProvider = services.BuildServiceProvider();
    }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        // Test several canceled connection attempts up front to minimize the number of times the user needs
        // to click OK on the Dorico connection dialog.
        var source = new CancellationTokenSource();
        source.Cancel();

        var connArgsNoWebSocketConnectMock = new Mock<IConnectionArguments>();
        connArgsNoWebSocketConnectMock.Setup(x => x.SessionToken).Returns((string?)null);
        connArgsNoWebSocketConnectMock.Setup(x => x.Address).Returns("ws://127.0.0.1:4560");
        connArgsNoWebSocketConnectMock.Setup(x => x.HandshakeVersion).Returns("1.0");
        connArgsNoWebSocketConnectMock.Setup(x => x.SessionToken).Returns((string?)null);
        connArgsNoWebSocketConnectMock.Setup(x => x.CancellationToken).Returns(source.Token);

        var connArgsNoConnectMock = new Mock<IConnectionArguments>();
        connArgsNoConnectMock.Setup(x => x.SessionToken).Returns((string?)null);
        connArgsNoConnectMock.Setup(x => x.Address).Returns("ws://127.0.0.1:4560");
        connArgsNoConnectMock.Setup(x => x.HandshakeVersion).Returns("1.0");
        connArgsNoConnectMock.SetupSequence(x => x.CancellationToken)
            .Returns(CancellationToken.None)
            .Returns(source.Token)
            .Returns(CancellationToken.None)
            .Returns(source.Token);

        var connArgsNoTokenReturnedMock = new Mock<IConnectionArguments>();
        connArgsNoTokenReturnedMock.Setup(x => x.SessionToken).Returns((string?)null);
        connArgsNoTokenReturnedMock.Setup(x => x.Address).Returns("ws://127.0.0.1:4560");
        connArgsNoTokenReturnedMock.Setup(x => x.HandshakeVersion).Returns("1.0");
        connArgsNoTokenReturnedMock.SetupSequence(x => x.CancellationToken)
            .Returns(CancellationToken.None)
            .Returns(CancellationToken.None)
            .Returns(source.Token);

        if (Process.GetProcessesByName(DoricoFileName).Length == 0)
        {
            Console.WriteLine("Cannot run integration tests. Make sure Dorico is open and a blank project is loaded.");
            return;
        }

        _remote = _serviceProvider.GetService<IDoricoRemote>()!;

        if (string.IsNullOrWhiteSpace(_sessionToken))
        {
            // Test cancel opening WebSocket
            Assert.ThrowsAsync<DoricoException>(async () =>
                await _remote.ConnectAsync(_clientName, connArgsNoWebSocketConnectMock.Object).ConfigureAwait(false));

            // test cancel connect without token
            Assert.ThrowsAsync<DoricoException>(async () =>
                await _remote.ConnectAsync(_clientName, connArgsNoConnectMock.Object).ConfigureAwait(false));

            await _remote.DisconnectAsync();
            await Task.Delay(100);

            // test cancel receiving token
            Assert.ThrowsAsync<DoricoException>(async () =>
                await _remote.ConnectAsync(_clientName, connArgsNoTokenReturnedMock.Object).ConfigureAwait(false));

            await _remote.DisconnectAsync();
            await Task.Delay(100);

            Assert.That(_remote.IsConnected, Is.False);
            Assert.ThrowsAsync<DoricoNotConnectedException>(() => _remote.GetAppInfoAsync());

            // connect for real
            if (!await _remote.ConnectAsync(_clientName, new ConnectionArguments()))
            {
                Console.WriteLine("Unable to connect to Dorico.");
                return;
            }
            Assert.That(_remote.IsConnected, Is.True);

            _sessionToken = _remote.SessionToken;

            await _remote.DisconnectAsync();
            await Task.Delay(100);
        }

        Assert.That(_remote.IsConnected, Is.False);

        // test cancel connect with token
        connArgsNoConnectMock.Setup(x => x.SessionToken).Returns(_sessionToken);
        Assert.ThrowsAsync<DoricoException>(async () =>
            await _remote.ConnectAsync(_clientName, connArgsNoConnectMock.Object).ConfigureAwait(false));

        if (_remote.IsConnected)
        {
            await _remote.DisconnectAsync();
            await Task.Delay(100);
        }

        if (!await _remote.ConnectAsync("Dorico.Net Integration Test", new ConnectionArguments() { SessionToken = _sessionToken }))
        {
            Console.WriteLine("Unable to connect to Dorico with session token.");
            return;
        }

        Assert.Multiple(() =>
        {
            Assert.That(_remote.IsConnected, Is.True);
            Assert.That(_remote.ClientName, Is.EqualTo(_clientName));
            Assert.That(_remote.SessionToken, Is.EqualTo(_sessionToken));
        });
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        var disconnected = await _remote.DisconnectAsync();
        await Task.Delay(100);

        Assert.Multiple(() =>
        {
            Assert.That(disconnected, Is.True);
            Assert.That(_remote.IsConnected, Is.False);
        });
    }

    [Test, Order(-2)]
    public void GetStatusCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.GetStatusAsync(source.Token).ConfigureAwait(false));
    }

    [Test, Order(-1)]
    public async Task GetStatus()
    {
        StatusResponse? publishedResponse = null;

        void OnStatusUpdate(IEvent statusResponse)
        {
            publishedResponse = statusResponse as StatusResponse;
        }

        _serviceProvider.GetService<IEventAggregator>()?.Subscribe<StatusResponse>(OnStatusUpdate);

        var response = await _remote.GetStatusAsync().ConfigureAwait(false);

        await Task.Delay(100);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.EqualTo(publishedResponse));
        });
    }

    [Test, Order(1)]
    public async Task GetAppInfo()
    {
        var response = await _remote.GetAppInfoAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response?.ToString().Length, Is.GreaterThan(2));
            Assert.That(response?.Variant, Is.Not.Null);
            Assert.That(response?.Number, Is.Not.Null);
            Assert.That(response?.RawJson, Is.Not.Null);
            Assert.That(response?.Message, Is.EqualTo("version"));
        });
    }

    [Test, Order(2)]
    public async Task GetAppInfoCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        try
        {
            _ = await _remote.GetAppInfoAsync(source.Token);
        }
        catch (DoricoException ex)
        {
            Assert.That(ex.Message, Is.EqualTo("Request was canceled or timed out."));
        }
        catch
        {
            Assert.Fail();
        }
    }

    [Test, Order(3)]
    public void GetCommandCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.GetCommandAsync("File.Open", source.Token));
    }

    [Test, Order(4)]
    public async Task GetCommand()
    {
        var command = await _remote.GetCommandAsync("NoteEdit.Pitch");

        Assert.ThrowsAsync<InvalidOperationException>(async () => await _remote.GetCommandAsync("Nope"));

        Assert.Multiple(() =>
        {
            Assert.That(command, Is.Not.Null);
            Assert.That(command?.ToString().Length, Is.GreaterThan(2));
            Assert.That(command?.Name, Is.Not.Null);
            Assert.That(command?.DisplayName, Is.Not.Null);
            Assert.That(command?.RequiredParameters?.Any(), Is.True);
            Assert.That(command?.OptionalParameters?.Any(), Is.True);
        });
    }

    [Test, Order(5)]
    public async Task GetCommandCancel2()
    {
        // since the commands are already loaded, there should be no SendAsync call to cancel

        var source = new CancellationTokenSource();
        source.Cancel();
        var command = await _remote.GetCommandAsync("File.Open", source.Token);

        Assert.That(command, Is.Not.Null);
    }

    [Test, Order(6)]
    public void GetEngravingOptionsCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.GetEngravingOptionsAsync(source.Token).ConfigureAwait(false));
    }

    [Test, Order(7)]
    public async Task GetEngravingOptions()
    {
        var options = await _remote.GetEngravingOptionsAsync().ConfigureAwait(false);
        var option = options!["dynamicsOptions.textBasedDynamicHorizontalAlignment"];
        var tryResult = options!.TryGetValue("Nope", out var tryOption);

        Assert.Throws<KeyNotFoundException>(() => _ = options!["Nope"]);

        Assert.Multiple(() =>
        {
            Assert.That(options, Is.Not.Null);
            Assert.That(tryResult, Is.False);
            Assert.That(tryOption, Is.Null);
        });
    }

    [Test, Order(9)]
    public void GetFlowsCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.GetFlowsAsync(source.Token).ConfigureAwait(false));
    }

    [Test, Order(10)]
    public async Task GetFlows()
    {
        var response = await _remote.GetFlowsAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Flows.Any(), Is.True);
        });
    }

    [Test, Order(11)]
    public void GetLayoutOptionsCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.GetLayoutOptionsAsync(0, source.Token).ConfigureAwait(false));
    }

    [Test, Order(12)]
    public async Task GetLayoutOptions()
    {
        var response = await _remote.GetLayoutOptionsAsync(0).ConfigureAwait(false);

        try
        {
            await _remote.GetLayoutOptionsAsync(-1);
        }
        catch (DoricoException<Response> ex)
        {
            Assert.That(ex.Response.Code, Is.EqualTo("kError"));
        }

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Any(), Is.True);
        });
    }

    [Test, Order(13)]
    public void GetLayoutsCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.GetLayoutsAsync(source.Token).ConfigureAwait(false));
    }

    [Test, Order(14)]
    public async Task GetLayouts()
    {
        var response = await _remote.GetLayoutsAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Layouts.Any(), Is.True);
        });
    }

    [Test, Order(15)]
    public void GetLibraryCollectionsCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.GetLibraryCollectionsAsync(source.Token).ConfigureAwait(false));
    }

    [Test, Order(16)]
    public void GetLibraryEntitiesCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.GetLibraryEntitiesAsync("", source.Token).ConfigureAwait(false));
    }

    [Test, Order(17)]
    public async Task GetLibraryCollectionsAndEntities()
    {
        var collectionsResponse = await _remote.GetLibraryCollectionsAsync().ConfigureAwait(false);
        var entitiesResponse = await _remote.GetLibraryEntitiesAsync(collectionsResponse!.Collections.First()).ConfigureAwait(false);
        var nopeResponse = await _remote.GetLibraryEntitiesAsync("Nope").ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(collectionsResponse, Is.Not.Null);
            Assert.That(collectionsResponse!.Collections.Any(), Is.True);
            Assert.That(entitiesResponse, Is.Not.Null);
            Assert.That(entitiesResponse!.Any(), Is.True);
            Assert.That(nopeResponse, Is.Not.Null);
            Assert.That(nopeResponse!.Any(), Is.False);
        });
    }

    [Test, Order(18)]
    public void GetNotationOptionsCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.GetNotationOptionsAsync(0, source.Token).ConfigureAwait(false));
    }

    [Test, Order(19)]
    public async Task GetNotationOptions()
    {
        var response = await _remote.GetNotationOptionsAsync(0).ConfigureAwait(false);

        Assert.ThrowsAsync<DoricoException<Response>>(async () => await _remote.GetNotationOptionsAsync(-1));

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Any(), Is.True);
        });
    }

    [Test, Order(20)]
    public void GetPlaybackTechniquesCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.GetPlaybackTechniquesAsync(source.Token).ConfigureAwait(false));
    }

    [Test, Order(21)]
    public async Task GetPlaybackTechniques()
    {
        var response = await _remote.GetPlaybackTechniquesAsync().ConfigureAwait(false);
        Assert.That(response, Is.Not.Null);
    }

    [Test, Order(24)]
    public void SetLayoutOptionsCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.SetLayoutOptionsAsync(new List<OptionValue>(), null, source.Token).ConfigureAwait(false));
    }

    [Test, Order(25)]
    public async Task SetLayoutOptions()
    {
        var values = new List<OptionValue>
        {
            new("barNumberLayoutOptions.barNumberFrequency", "kEveryBar")
        };
        var response = await _remote.SetLayoutOptionsAsync(values, new[] { 1, 2, 3 }).ConfigureAwait(false);
        var optionInfo = (await _remote.GetLayoutOptionsAsync(0))?["barNumberLayoutOptions.barNumberFrequency"];

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(optionInfo?.CurrentValue, Is.EqualTo("kEveryBar"));
        });
    }

    [Test, Order(32)]
    public void SetLayoutOptionsLayoutIdCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.SetLayoutOptionsAsync(new List<OptionValue>(), LayoutIds.kAllFullScoreLayouts, source.Token).ConfigureAwait(false));
    }

    [Test, Order(26)]
    public async Task SetLayoutOptionsLayoutId()
    {
        var values = new List<OptionValue>
        {
            new("barNumberLayoutOptions.barNumberFrequency", "kNone")
        };
        var response = await _remote.SetLayoutOptionsAsync(values, LayoutIds.kAllFullScoreLayouts).ConfigureAwait(false);

        await Task.Delay(100);

        var optionInfo = (await _remote.GetLayoutOptionsAsync(0))?["barNumberLayoutOptions.barNumberFrequency"];

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(optionInfo?.CurrentValue, Is.EqualTo("kNone"));
        });
    }

    [Test, Order(27)]
    public void SetNotationOptionsCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.SetNotationOptionsAsync(new List<OptionValue>(), null, source.Token).ConfigureAwait(false));
    }

    [Test, Order(28)]
    public async Task SetNotationOptions()
    {
        var values = new List<OptionValue>
        {
            new("barlineOptions.defaultBarlineType", "kDouble")
        };
        var response = await _remote.SetNotationOptionsAsync(values, new[] { 0 }).ConfigureAwait(false);
        var optionInfo = (await _remote.GetNotationOptionsAsync(0))?["barlineOptions.defaultBarlineType"];

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(optionInfo?.CurrentValue, Is.EqualTo("kDouble"));
        });
    }

    [Test, Order(33)]
    public void SetNotationOptionsFlowIdCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.SetNotationOptionsAsync(new List<OptionValue>(), FlowIds.kAll, source.Token).ConfigureAwait(false));
    }

    [Test, Order(29)]
    public async Task SetNotationOptionsFlowId()
    {
        var values = new List<OptionValue>
        {
            new("barlineOptions.finalBarlineType", "kThick")
        };
        var response = await _remote.SetNotationOptionsAsync(values, FlowIds.kAll).ConfigureAwait(false);
        var optionInfo = (await _remote.GetNotationOptionsAsync(0))?["barlineOptions.finalBarlineType"];

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(optionInfo?.CurrentValue, Is.EqualTo("kThick"));
        });
    }

    [Test, Order(30)]
    public void SetEngravingOptionsCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.SetEngravingOptionsAsync(new List<OptionValue>(), source.Token).ConfigureAwait(false));
    }

    [Test, Order(31)]
    public async Task SetEngravingOptions()
    {
        var values = new List<OptionValue>
        {
            new("glissandoLineOptions.glissandoLineText", "kNoText")
        };
        var response = await _remote.SetEngravingOptionsAsync(values).ConfigureAwait(false);
        var optionInfo = (await _remote.GetEngravingOptionsAsync())?["glissandoLineOptions.glissandoLineText"];

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(optionInfo?.CurrentValue, Is.EqualTo("kNoText"));
        });
    }

    [Test, Order(48)]
    public void SendRequestCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.SendRequestAsync(new GetAppInfoRequest(), source.Token).ConfigureAwait(false));
    }

    [Test, Order(49)]
    public void GetPropertiesCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.GetPropertiesAsync(source.Token).ConfigureAwait(false));
    }

    [Test, Order(50)]
    public async Task GetProperties()
    {
        var cmdResponse = (Response?)await _remote.SendRequestAsync(new Command("Window.SwitchMode?WindowMode=kWriteMode"));
        Assert.That(cmdResponse!.Code, Is.EqualTo("kOK"));

        await InsertNoteAsync(new Note("F#", 4));

        var response = await _remote.GetPropertiesAsync().ConfigureAwait(false);
        Assert.That(response?.Properties?.Any(), Is.True);
    }

    [Test, Order(200)]
    public void DisconnectCancel()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        Assert.ThrowsAsync<DoricoException>(async () => await _remote.DisconnectAsync(source.Token).ConfigureAwait(false));
    }

    private async Task InsertNoteAsync(Note note)
    {
        await _remote!.SendRequestAsync(new Command("NoteInput.Exit"));
        await _remote!.SendRequestAsync(new Command("NoteInput.Enter"));

        // Since Dorico doesn't set the accidental as part of the pitch, the Note class
        // has a helper that return the commands required to set the note.
        foreach (var command in note.GetNoteCommands())
        {
            await _remote!.SendRequestAsync(command);
        }

        await _remote!.SendRequestAsync(new Command("NoteInput.Exit"));
    }
}