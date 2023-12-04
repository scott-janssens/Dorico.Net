namespace DoricoNet.Requests;

public record OptionValue(string Path, string Value)
{
    public string RequestTemplate => $"{{\"path\": \"{Path}\", \"value\": \"{Value}\" }}";
}
