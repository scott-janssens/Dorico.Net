namespace DoricoNet.Commands;

/// <summary>
/// Parameter information for a command.
/// </summary>
/// <param name="Name">The name of the parameter.</param>
/// <param name="Value">The current value of the parameter.</param>
public record CommandParameter(string Name, string Value)
{
    /// <inheritdoc/>
    public override string ToString() => $"{Name}={Value}";
}
