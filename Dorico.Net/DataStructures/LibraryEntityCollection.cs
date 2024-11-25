using CommunityToolkit.Diagnostics;
using DoricoNet.Responses;
using System.Collections;
using System.Collections.Immutable;

namespace DoricoNet.DataStructures;

/// <summary>
/// Organized list of LibraryEntity objects
/// </summary>
public class LibraryEntityCollection : ICollection<LibraryEntity>
{
    private readonly Dictionary<string, LibraryEntity> _byId = new();
    private readonly Dictionary<string, List<LibraryEntity>> _byName = new();

    /// <inheritdoc/>
    public int Count => _byId.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// Returns a library entity with the specified EntityId.
    /// </summary>
    /// <param name="entityId">The EntityId of an entity.</param>
    /// <returns>A LibraryEntity object.</returns>
    public LibraryEntity GetByEntityId(string entityId) => _byId[entityId];

    /// <summary>
    /// Returns a library entity with the specified EntityId.
    /// </summary>
    /// <param name="name">The name of an entity.</param>
    /// <returns>A LibraryEntity object.</returns>
    public IImmutableList<LibraryEntity> GetByName(string name) => _byName[name].ToImmutableList();

    /// <inheritdoc/>
    public void Add(LibraryEntity item)
    {
        Guard.IsNotNull(item, nameof(item));

        _byId.Add(item.EntityId, item);

        if (!_byName.TryGetValue(item.Name, out var list))
        {
            list = new();
            _byName.Add(item.Name, list);
        }

        list.Add(item);
    }

    /// <inheritdoc/>
    public void Clear() => throw new NotSupportedException();

    /// <inheritdoc/>
    public bool Contains(LibraryEntity item)
    {
        Guard.IsNotNull(item, nameof(item));
        return _byId.ContainsKey(item.EntityId);
    }

    /// <inheritdoc/>
    public void CopyTo(LibraryEntity[] array, int arrayIndex) => throw new NotSupportedException();

    /// <inheritdoc/>
    public bool Remove(LibraryEntity item)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public IEnumerator<LibraryEntity> GetEnumerator() => _byId.Values.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => _byId.Values.GetEnumerator();
}
