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
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>true if disconnected from Dorico, otherwise false</returns>
    Task<bool> DisconnectAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Requests information about the instance of Dorico.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>A VersionResponse object.</returns>
    Task<VersionResponse?> GetAppInfoAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Retrieves the CommandInfo object for a command with the specified name.
    /// </summary>
    /// <param name="name">The name of a command.</param>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>A CommandInfo object.</returns>
    Task<CommandInfo> GetCommandAsync(string name, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Retrieves and caches all of Dorico's commands.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>A CommandCollection object.</returns>
    Task<CommandCollection> GetCommandsAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Returns Dorico's engraving options and their current values.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>An OptionCollection object.</returns>
    Task<OptionCollection?> GetEngravingOptionsAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Retrieves information about the flows in the currently active score.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>A FlowsListResponse object.</returns>
    Task<FlowsListResponse?> GetFlowsAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Retrieves the layout options for the specified layout.
    /// </summary>
    /// <param name="layoutID">The ID of a layout.</param>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>An OptionCollection object.</returns>
    Task<OptionCollection?> GetLayoutOptionsAsync(int layoutID, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Retrieves information about the layouts in the currently active score.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>A LayoutsListResponse object.</returns>
    Task<LayoutsListResponse?> GetLayoutsAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Retrieves a list of the library collections for the active score.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>A LibraryCollectionsListResponse object.</returns>
    Task<LibraryCollectionsListResponse?> GetLibraryCollectionsAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Retrieves a list of the entities in the specified collection for the active score
    /// </summary>
    /// <param name="collection">The collection to query.</param>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>A LibraryEntityCollection object.</returns>
    Task<LibraryEntityCollection?> GetLibraryEntitiesAsync(string collection, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Retrieves the notation options for the specified flow.
    /// </summary>
    /// <param name="flowID">The ID of a flow.</param>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>An OptionCollection object</returns>
    Task<OptionCollection?> GetNotationOptionsAsync(int flowID, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Retrieves a list of the techniques defined in the expression map of the playback endpoint of the top most 
    /// instrument in the current selection.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>A PlaybackTechniquesListResponse object.</returns>
    Task<PlaybackTechniquesListResponse?> GetPlaybackTechniquesAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Retrieves information about the properties which can be set on the current selection.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>A PropertiesListResponse object.</returns>
    Task<PropertiesListResponse?> GetPropertiesAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Retrieves Dorico's current status.
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>A StatusResponse object.</returns>
    Task<StatusResponse?> GetStatusAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Set the specified layout option values for the layouts whose IDs are are in the layoutIds collection.
    /// </summary>
    /// <param name="optionValues">A collection of OptionValue objects.</param>
    /// <param name="layoutIds">A collection of layout IDs to affect.</param>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns></returns>
    Task<Response?> SetLayoutOptionsAsync(IEnumerable<OptionValue> optionValues, IEnumerable<int>? layoutIds = null, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Set the specified layout option values for the layouts which satisfy the LayoutIds enum value.
    /// </summary>
    /// <param name="optionValues">A collection of OptionValue objects.</param>
    /// <param name="layoutIds">A LayoutIDs enum value.</param>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns></returns>
    Task<Response?> SetLayoutOptionsAsync(IEnumerable<OptionValue> optionValues, LayoutIds layoutIds, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Set the specified notation option values for the flows whose IDs are are in the layoutIds collection.
    /// </summary>
    /// <param name="optionValues">A collection of OptionValue objects.</param>
    /// <param name="flowIds">A collection of flow IDs to affect.</param>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>A Response object.</returns>
    Task<Response?> SetNotationOptionsAsync(IEnumerable<OptionValue> optionValues, IEnumerable<int>? flowIds = null, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Set the specified notation option values for the flows which satisfy the FlowIds enum value.
    /// </summary>
    /// <param name="optionValues">A collection of OptionValue objects.</param>
    /// <param name="flowIds">A FlowId enum value.</param>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>A Response object.</returns>
    Task<Response?> SetNotationOptionsAsync(IEnumerable<OptionValue> optionValues, FlowIds flowIds, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Set the specified engraving option values.
    /// </summary>
    /// <param name="optionValues">A collection of OptionValue objects.</param>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>A Response object.</returns>
    Task<Response?> SetEngravingOptionsAsync(IEnumerable<OptionValue> optionValues, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Sends a request to Dorico.
    /// </summary>
    /// <param name="request">A request object</param>
    /// <param name="cancellationToken">A CancellationToken object.</param>
    /// <returns>A response object.</returns>
    Task<IDoricoResponse?> SendRequestAsync(IDoricoRequest request, CancellationToken? cancellationToken = null);
}