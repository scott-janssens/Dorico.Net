namespace DoricoNet.Comms;

public interface IConnectionArguments
{
    string Address { get; init; }
    CancellationToken? CancellationToken { get; init; }
    string HandshakeVersion { get; init; }
    string? SessionToken { get; init; }
}

/// <summary>
/// Dorico connection information.
/// </summary>
/// <param name="SessionToken">Session token if previously connected to Dorcio, otherwise null.</param>
/// <param name="Address">Address of Dorico's web socket.</param>
/// <param name="HandshakeVersion">Handshake version that Dorico is using</param>
/// <param name="CancellationToken">Cancellation token</param>
public record ConnectionArguments(
    string? SessionToken = null,
    string Address = "ws://127.0.0.1:4560",
    string HandshakeVersion = "1.0",
    CancellationToken? CancellationToken = null) : IConnectionArguments;
