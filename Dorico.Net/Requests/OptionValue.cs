namespace DoricoNet.Requests;

/// <summary>
/// Details on an individual property
/// </summary>
/// <param name="Path">The path to this option</param>
/// <param name="Value">The current value of the option</param>
public record OptionValue(string Path, string Value)
{
    public string RequestTemplate => $"{{\"path\": \"{Path}\", \"value\": \"{Value}\" }}";
}
