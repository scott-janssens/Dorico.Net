using DoricoNet.DataStructures;

namespace DoricoNet.Responses;

/// <summary>
/// Details on an individual command
/// </summary>
/// <param name="Name">The name of the command</param>
/// <param name="DisplayName">A human readable name for the command</param>
/// <param name="RequiredParameters">Parameters required by the command</param>
/// <param name="OptionalParameters">Optional parameters for the command</param>
public sealed record CommandInfo(
    string Name,
    string? DisplayName = null,
    IEnumerable<string>? RequiredParameters = null,
    IEnumerable<string>? OptionalParameters = null) : IOrganizable
{
    /// <summary>
    /// Parameters required by the command.
    /// </summary>
    public IEnumerable<string> RequiredParameters { get; } = RequiredParameters ?? [];

    /// <summary>
    /// Optional parameters for the command
    /// </summary>
    public IEnumerable<string> OptionalParameters { get; } = OptionalParameters ?? [];

    /// <inheritdoc/>
    public override string ToString() => DisplayName ?? Name;

    /// <inheritdoc/>
    Func<string> IOrganizable.GetNameValue => () => Name;
}
