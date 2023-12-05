using DoricoNet.Responses;

namespace DoricoNet.Requests;

/// <summary>
/// Requests a list of the library collections for the active score.
/// </summary>
public record GetLibraryCollectionsRequest : DoricoRequestBase<LibraryCollectionsListResponse>
{
    /// <inheritdoc/>
    public override string Message => "{\"message\": \"getlibrarycollections\"}";

    /// <inheritdoc/>
    public override string MessageId => "getlibrarycollections";
}
