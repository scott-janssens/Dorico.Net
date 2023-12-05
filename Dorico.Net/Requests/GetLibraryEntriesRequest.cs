using DoricoNet.Responses;

namespace DoricoNet.Requests;

/// <summary>
/// Requests a list of the entities in the specified collection for the active score.
/// </summary>
/// <param name="Collection">The collection to query.</param>
public record GetLibraryEntriesRequest(string Collection) : DoricoRequestBase<LibraryEntitiesListResponse>
{
    /// <inheritdoc/>
    public override string Message => $"{{\"message\": \"getlibraryentities\",\"collection\":\"{Collection}\"}}";

    /// <inheritdoc/>
    public override string MessageId => "getlibraryentities";
}
