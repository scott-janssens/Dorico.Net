using CommunityToolkit.Diagnostics;
using DoricoNet.Comms;
using DoricoNet.DataStructures;
using DoricoNet.Enums;
using DoricoNet.Exceptions;
using DoricoNet.Requests;
using DoricoNet.Responses;
using Lea;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;

namespace DoricoNet;

/// <summary>
/// Base class for controling Dorico remotely. It encapsulates all the core functionality 
/// of the Dorico remote API.  Specific functionality, such as executing particular commands
/// or a series of commands, is expected to be implemented in inheriting classes.
/// </summary>
public partial class DoricoRemote : IDoricoRemote
{
    private readonly IDoricoCommsContext _commsContext;
    private readonly IEventAggregator _eventAggregator;
    private readonly ILogger _logger;

    private CommandCollection? _commands;

    #region LoggerMessages

    [LoggerMessage(LogLevel.Information, "Dorico.NET - Connected to Dorico.")]
    static partial void LogConnection(ILogger logger);

    [LoggerMessage(LogLevel.Information, "Dorico.NET - Disconnected from Dorico")]
    static partial void LogDisconnect(ILogger logger);

    [LoggerMessage(LogLevel.Error, "Dorico.NET - Could not connect to Dorico. Make sure Dorico is running.")]
    static partial void LogConnectionError(ILogger logger);

    [LoggerMessage(LogLevel.Trace, "Dorico.NET - Status:\n{Status}")]
    static partial void LogStatus(ILogger logger, string status);

    #endregion

    public string? ClientName { get; protected set; }

    public bool IsConnected => _commsContext.State == WebSocketState.Open;

    public string? SessionToken { get; protected set; }

    public int Timeout { get; set; } = 30000;

    public DoricoRemote(IDoricoCommsContext commsContext, IEventAggregator eventAggregator, ILogger logger)
    {
        _commsContext = commsContext;
        _eventAggregator = eventAggregator;
        _logger = logger;
    }

    public virtual async Task<bool> ConnectAsync(string clientName, IConnectionArguments connectionArguments)
    {
        Guard.IsNotNull(clientName);
        Guard.IsNotNull(connectionArguments, nameof(connectionArguments));

        ClientName = clientName;

        if (!IsConnected)
        {
            try
            {
                if (_commsContext.State != WebSocketState.Open)
                {
                    await _commsContext.ConnectAsync(connectionArguments).ConfigureAwait(false);
                }

                if (_commsContext.State != WebSocketState.Open)
                {
                    LogConnectionError(_logger);
                    throw new DoricoException("Could not connect to Dorico. Make sure Dorico is running.");
                }
                else
                {
                    if (connectionArguments.SessionToken == null)
                    {
                        SessionToken = await ConnectWithoutSessionTokenAsync(connectionArguments).ConfigureAwait(false);
                    }
                    else
                    {
                        SessionToken = connectionArguments.SessionToken;
                        await ConnectAsyncInternal(connectionArguments).ConfigureAwait(false);
                    }
                }
            }
            catch (DoricoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DoricoException("Could not connect to Dorico. Make sure Dorico is running.", ex);
            }
        }

        if (IsConnected)
        {
            LogConnection(_logger);
        }

        return IsConnected;
    }

    public virtual async Task<bool> DisconnectAsync(CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        await _commsContext.StopAsync(cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        if (!IsConnected)
        {
            LogDisconnect(_logger);
        }

        return !IsConnected;
    }

    public virtual async Task<VersionResponse?> GetAppInfoAsync(CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new GetAppInfoRequest();
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return (VersionResponse?)response;
    }

    public virtual async Task<CommandCollection> GetCommandsAsync(CancellationToken? cancellationToken = null)
    {
        if (_commands == null || !_commands.Any())
        {
            AssertConnected();

            var request = new GetCommandsRequest();
            var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

            ErrorCheck(request, response);

            _commands = ((CommandListResponse?)response)?.Commands;
        }

        return _commands!;
    }

    public virtual async Task<CommandInfo> GetCommandAsync(string name, CancellationToken? cancellationToken = null)
    {
        // AssertConnected(); called in GetCommandsAsync()
        await GetCommandsAsync(cancellationToken).ConfigureAwait(false);

        CommandInfo? command = null;
        _commands?.TryGetValue(name, out command);
        return command ?? throw new InvalidOperationException($"No command with name '{name}' found.");
    }

    public async Task<OptionCollection?> GetEngravingOptionsAsync(CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new GetOptionsRequest(OptionsType.kEngraving);
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return ((OptionsListResponse?)response)?.Options;
    }

    public async Task<OptionCollection?> GetLayoutOptionsAsync(int layoutID, CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new GetOptionsRequest(OptionsType.kLayout, layoutID);
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return ((OptionsListResponse?)response)?.Options;
    }

    public async Task<OptionCollection?> GetNotationOptionsAsync(int flowID, CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new GetOptionsRequest(OptionsType.kNotation, flowID);
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return ((OptionsListResponse?)response)?.Options;
    }

    public virtual async Task<Response?> SetLayoutOptionsAsync(IEnumerable<OptionValue> optionValues, IEnumerable<int>? layoutIds = null, CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new SetLayoutOptionsRequest(optionValues, layoutIds);
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return (Response?)response;
    }

    public virtual async Task<Response?> SetLayoutOptionsAsync(IEnumerable<OptionValue> optionValues, LayoutIds layoutIds, CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new SetLayoutOptionsRequest(optionValues, layoutIds);
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return (Response?)response;
    }

    public virtual async Task<Response?> SetNotationOptionsAsync(IEnumerable<OptionValue> optionValues, IEnumerable<int>? flowIds = null, CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new SetNotationOptionsRequest(optionValues, flowIds);
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return (Response?)response;
    }

    public virtual async Task<Response?> SetNotationOptionsAsync(IEnumerable<OptionValue> optionValues, FlowIds flowIds, CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new SetNotationOptionsRequest(optionValues, flowIds);
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return (Response?)response;
    }

    public virtual async Task<Response?> SetEngravingOptionsAsync(IEnumerable<OptionValue> optionValues, CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new SetEngravingOptionsRequest(optionValues);
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return (Response?)response;
    }

    public virtual async Task<FlowsListResponse?> GetFlowsAsync(CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new GetFlowsRequest();
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return (FlowsListResponse?)response;
    }

    public virtual async Task<LayoutsListResponse?> GetLayoutsAsync(CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new GetLayoutsRequest();
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return (LayoutsListResponse?)response;
    }

    public virtual async Task<LibraryCollectionsListResponse?> GetLibraryCollectionsAsync(CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new GetLibraryCollectionsRequest();
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return (LibraryCollectionsListResponse?)response;
    }

    public virtual async Task<LibraryEntityCollection?> GetLibraryEntitiesAsync(string collection, CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new GetLibraryEntriesRequest(collection);
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return ((LibraryEntitiesListResponse?)response)!.LibraryEntities;
    }

    public virtual async Task<PlaybackTechniquesListResponse?> GetPlaybackTechniquesAsync(CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new GetPlaybackTechniquesRequest();
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return (PlaybackTechniquesListResponse?)response;
    }

    public virtual async Task<PropertiesListResponse?> GetPropertiesAsync(CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new GetPropertiesRequest();
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return (PropertiesListResponse?)response;
    }

    public virtual async Task<StatusResponse?> GetStatusAsync(CancellationToken? cancellationToken = null)
    {
        AssertConnected();

        var request = new GetStatusRequest();
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return (StatusResponse?)response;
    }

    public virtual async Task<IDoricoResponse?> SendRequestAsync(IDoricoRequest request, CancellationToken? cancellationToken = null)
    {
        Guard.IsNotNull(request);

        AssertConnected();
        var response = await _commsContext.SendAsync(request, cancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(request, response);

        return response;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void AssertConnected()
    {
        if (!IsConnected)
        {
            throw new DoricoNotConnectedException();
        }
    }

    private async Task<string> ConnectWithoutSessionTokenAsync(IConnectionArguments connectionArguments)
    {
        var connectRequest = new ConnectRequest(ClientName!, connectionArguments.HandshakeVersion);

        await _commsContext.SendAsync(connectRequest, connectionArguments.CancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        if (connectRequest.TypedResponse == null || string.IsNullOrWhiteSpace(connectRequest.TypedResponse.SessionToken))
        {
            throw new InvalidOperationException("No sessionToken returned");
        }

        var acceptSessionTokenRequest = new AcceptSessionTokenRequest(connectRequest.TypedResponse.SessionToken);
        var response = await _commsContext.SendAsync(acceptSessionTokenRequest, connectionArguments.CancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(acceptSessionTokenRequest, response);

        return connectRequest.TypedResponse.SessionToken;
    }

    private async Task ConnectAsyncInternal(IConnectionArguments connectionArguments)
    {
        Guard.IsNotNull(connectionArguments.SessionToken, nameof(connectionArguments.SessionToken));

        var connectRequest = new ConnectWithSessionRequest(ClientName!, connectionArguments.SessionToken, connectionArguments.HandshakeVersion);

        var response = await _commsContext.SendAsync(connectRequest, connectionArguments.CancellationToken ?? CancellationToken.None, Timeout).ConfigureAwait(false);

        ErrorCheck(connectRequest, response);

        if (connectRequest.TypedResponse == null || connectRequest.TypedResponse.Code != "kConnected")
        {
            throw new InvalidOperationException($"Unable to connect with sessionToken {connectionArguments.SessionToken}");
        }
    }

    private static void ErrorCheck(IDoricoRequest request, IDoricoResponse? response)
    {
        if (request.IsAborted)
        {
            throw new DoricoException("Request was cancelled or timed out.");
        }

        if (response != null && response is Response errorResponse && errorResponse.Code == "kError")
        {
            throw new DoricoException<Response>(errorResponse, errorResponse.Detail!);
        }
    }
}
