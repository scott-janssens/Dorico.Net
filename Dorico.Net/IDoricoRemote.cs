using DoricoNet.Comms;
using DoricoNet.DataStructures;
using DoricoNet.Enums;
using DoricoNet.Requests;
using DoricoNet.Responses;

namespace DoricoNet;

/// <summary>
/// Class for interacting with the Dorico Remote Control API
/// </summary>
public interface IDoricoRemote
{
    /// <summary>
    /// The name presented to the user in Dorico when asking whether communication should be allowed.
    /// </summary>
    string? ClientName { get; }

    /// <summary>
    /// True if currently connected to Dorico, otherwise false. 
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Session token previously provided by Dorico. If valid Dorico will accept this connection without prompting the user.
    /// </summary>
    string? SessionToken { get; }

    /// <summary>
    /// Default timeout for calls to Dorico in milliseconds.
    /// </summary>
    int Timeout { get; set; }

    /// <summary>
    /// Opens communication with Dorico.
    /// </summary>
    /// <param name="connectionArguments">A ConnectionArguments object</param>
    /// <returns>true if connected to Dorico, otherwise false</returns>
    Task<bool> ConnectAsync(string clientName, IConnectionArguments connectionArguments);

    /// <summary>
    /// Disconnect from Dorico.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken object</param>
    /// <returns>true if disconnected from Dorico, otherwise false</returns>
    Task<bool> DisconnectAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Requests information about the instance of Dorico.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<VersionResponse?> GetAppInfoAsync(CancellationToken? cancellationToken = null);
    Task<CommandInfo> GetCommandAsync(string name, CancellationToken? cancellationToken = null);
    Task<CommandCollection> GetCommandsAsync(CancellationToken? cancellationToken = null);
    Task<OptionCollection?> GetEngravingOptionsAsync(CancellationToken? cancellationToken = null);
    Task<FlowsListResponse?> GetFlowsAsync(CancellationToken? cancellationToken = null);
    Task<OptionCollection?> GetLayoutOptionsAsync(int layoutID, CancellationToken? cancellationToken = null);
    Task<LayoutsListResponse?> GetLayoutsAsync(CancellationToken? cancellationToken = null);
    Task<LibraryCollectionsListResponse?> GetLibraryCollectionsAsync(CancellationToken? cancellationToken = null);
    Task<LibraryEntityCollection?> GetLibraryEntitiesAsync(string collection, CancellationToken? cancellationToken = null);
    Task<OptionCollection?> GetNotationOptionsAsync(int flowID, CancellationToken? cancellationToken = null);
    Task<PlaybackTechniquesListResponse?> GetPlaybackTechniquesAsync(CancellationToken? cancellationToken = null);
    Task<PropertiesListResponse?> GetPropertiesAsync(CancellationToken? cancellationToken = null);
    Task<StatusResponse?> GetStatusAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Instructs Dorico to set the specified option values
    /// </summary>
    /// <param name="optionsType">The type of options to set</param>
    /// <param name="path">The path to this option</param>
    /// <param name="value">The new value for this option</param>
    /// <param name="ids">Collection of flow IDs or layout IDs. If this property is not specified then the active layout or flow ID will be used</param>
    /// <returns></returns>
    Task<Response?> SetLayoutOptionsAsync(IEnumerable<OptionValue> optionValues, IEnumerable<int>? layoutIds = null, CancellationToken? cancellationToken = null);
    Task<Response?> SetLayoutOptionsAsync(IEnumerable<OptionValue> optionValues, LayoutIds layoutIds, CancellationToken? cancellationToken = null);
    Task<Response?> SetNotationOptionsAsync(IEnumerable<OptionValue> optionValues, IEnumerable<int>? flowIds = null, CancellationToken? cancellationToken = null);
    Task<Response?> SetNotationOptionsAsync(IEnumerable<OptionValue> optionValues, FlowIds flowIds, CancellationToken? cancellationToken = null);
    Task<Response?> SetEngravingOptionsAsync(IEnumerable<OptionValue> optionValues, CancellationToken? cancellationToken = null);
    Task<IDoricoResponse?> SendRequestAsync(IDoricoRequest request, CancellationToken? cancellationToken = null);
}