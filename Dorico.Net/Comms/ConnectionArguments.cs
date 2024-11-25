namespace DoricoNet.Comms;

/// <summary>
/// Dorico connection information.
/// </summary>
public interface IConnectionArguments
{
    /// <summary>
    /// Address of Dorico's web socket.
    /// </summary>
    string Address { get; init; }

    /// <summary>
    /// Cancellation token object
    /// </summary>
    CancellationToken? CancellationToken { get; init; }

    /// <summary>
    /// Handshake version that Dorico is using
    /// </summary>
    string HandshakeVersion { get; init; }

    /// <summary>
    /// Session token if previously connected to Dorico, otherwise null.
    /// </summary>
    string? SessionToken { get; init; }
}

/// <summary>
/// Dorico connection information.
/// </summary>
/// <param name="SessionToken">Session token if previously connected to Dorcio, otherwise null.</param>
/// <param name="Address">Address of Dorico's web socket.</param>
/// <param name="HandshakeVersion">Handshake version that Dorico is using</param>
/// <param name="CancellationToken">Cancellation token object</param>
public record ConnectionArguments(
    string? SessionToken = null,
    string Address = "ws://127.0.0.1:4560",
    string HandshakeVersion = "1.0",
    CancellationToken? CancellationToken = null) : IConnectionArguments;
