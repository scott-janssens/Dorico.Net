using DoricoNet.Responses;

namespace DoricoNet.Requests;

/// <summary>
/// Facilitates a request made to the Dorico Remote Control API.
/// </summary>
public interface IDoricoRequest
{
    /// <summary>
    /// The message send to Dorico.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// The ID of the request
    /// </summary>
    string MessageId { get; }

    /// <summary>
    /// True if the request has been aborted (canceled or timed out), otherwise false.
    /// </summary>
    bool IsAborted { get; }

    /// <summary>
    /// The type of response object associated with this request.
    /// </summary>
    Type ResponseType { get; }

    /// <summary>
    /// A response object if the request has completed, or null until that time.
    /// </summary>
    IDoricoResponse? Response { get; }

    /// <summary>
    /// A Response object with error information if Dorico returned an error, otherwise false.
    /// </summary>
    Response? ErrorResponse { get; }
}

/// <summary>
/// The response object cast to its actual type.
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface IDoricoRequest<out TResponse> : IDoricoRequest
{
    TResponse? TypedResponse { get; }
}