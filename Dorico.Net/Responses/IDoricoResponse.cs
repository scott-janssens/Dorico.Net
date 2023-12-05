namespace DoricoNet.Responses
{
    /// <summary>
    /// Contains the information from a Dorico response.
    /// </summary>
    public interface IDoricoResponse
    {
        /// <summary>
        /// The message from Dorico.
        /// </summary>
        string Message { get; init; }

        /// <summary>
        /// The raw JSON response from Dorico.
        /// </summary>
        string? RawJson { get; set; }
    }
}