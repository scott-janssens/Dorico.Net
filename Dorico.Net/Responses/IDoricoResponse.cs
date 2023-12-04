namespace DoricoNet.Responses
{
    public interface IDoricoResponse
    {
        string Message { get; init; }
        string? RawJson { get; set; }
    }
}