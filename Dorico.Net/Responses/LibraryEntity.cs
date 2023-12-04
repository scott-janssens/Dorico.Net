using DoricoNet.DataStructures;

namespace DoricoNet.Responses;

/// <summary>
/// Details on an individual entity
/// </summary>
/// <param name="EntityId">The ID of this entity</param>
/// <param name="Name">The name of this entity</param>
/// <param name="PlaybackTechniqueId">An associated playback technique ID, if there is one for this entity type</param>
public sealed record LibraryEntity(string EntityId, string Name, string? PlaybackTechniqueId) : IOrganizable
{
    Func<string> IOrganizable.GetNameValue => () => Name;
}
