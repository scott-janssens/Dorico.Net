using DoricoNet.Requests;
using DoricoNet.Responses;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;

namespace DoricoNet.Comms
{
    /// <summary>
    /// Abstraction for communicating with Dorico
    /// </summary>
    public interface IDoricoCommsContext
    {
        /// <summary>
        /// If true, the requests and responses will be written as Debug entries to the logger.
        /// </summary>
        bool Echo { get; set; }

        /// <summary>
        /// When Echo is true, messages types in HideMessageTypes will not be logged. For requests this is the value in
        /// IDoricoRequest.MessageId.  For responses this is the value in the ResponseMessage attribute.
        /// </summary>    
        Collection<string> HideMessageTypes { get; }

        /// <summary>
        /// True if the receive loop is executing, otherwise false
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// The state of the websocket connection with Dorico.
        /// </summary>
        WebSocketState State { get; }

        /// <summary>
        /// Opens connection with Dorico and starts receiving messages.
        /// </summary>
        /// <param name="connectionArgs">Dorico connection information</param>
        Task ConnectAsync(IConnectionArguments connectionArgs);

        /// <summary>
        /// Sends a request to the Dorico remote control API.
        /// </summary>
        /// <param name="request">Request object</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="timeout">timeout in milliseconds, defaults to -1 (infinity).</param>
        /// <returns>The response to the request.</returns>
        Task<IDoricoResponse?> SendAsync(IDoricoRequest request, CancellationToken cancellationToken, int timeout = -1);

        /// <summary>
        /// Starts receiving messages from Dorico.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops receiving messages from Dorico.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Name is not confusing")]
        [SuppressMessage("Design", "CA1068:CancellationToken parameters must come last", Justification = "Parameter order is consistent")]
        Task StopAsync(CancellationToken cancellationToken, int timeout = -1);
    }
}