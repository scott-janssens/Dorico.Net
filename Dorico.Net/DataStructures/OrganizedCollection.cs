using CommunityToolkit.Diagnostics;
using System.Collections;

namespace DoricoNet.DataStructures;

/// <summary>
/// Base class for organized lists.
/// </summary>
/// <typeparam name="T">Type of the items in the list</typeparam>
public abstract class OrganizedCollection<T> : ICollection<T>
    where T : IOrganizable
{
    private readonly Dictionary<string, T> _items = new();
    private OrganizedTreeListNode<T>? _organizedTreeRoot;

    /// <summary>
    /// Gets the item with the specified name.
    /// </summary>
    /// <param name="name">Name of the item to retrieve</param>
    /// <returns>The item with specified name if present, otherwise null.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when no item with has the specified name.</exception>
    public T this[string name] => _items[name];

    /// <summary>
    /// Attempts to retrieve the item with the specified name.
    /// </summary>
    /// <param name="name">Name of the item to retrieve</param>
    /// <param name="value">The returned item if present, otherwise null.</param>
    /// <returns>True if the item was found, otherwise false</returns>
    public bool TryGetValue(string name, out T? value)
    {
        _items.TryGetValue(name, out value);
        return value != null;
    }

    /// <summary>
    /// Returns the root node of a tree structure containing the organized items.
    /// </summary>
    public OrganizedTreeListNode<T>? OrganizedItems =>
        _organizedTreeRoot ??= new OrganizedTreeListNode<T>(_items.Values);

    /// <summary>
    /// Adds and item to the organized collection
    /// </summary>
    /// <param name="item">item object of type T</param>
    public void Add(T item)
    {
        Guard.IsNotNull(item, nameof(item));
        _items.Add(item.GetNameValue(), item);
    }

    /// <inheritdoc/>
    public int Count => _items.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => _items.Values.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => _items.Values.GetEnumerator();

    /// <inheritdoc/>
    public void Clear() => throw new NotSupportedException();

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        Guard.IsNotNull(item, nameof(item));
        return _items.ContainsKey(item.GetNameValue());
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex) => throw new NotSupportedException();

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        throw new NotSupportedException();
    }
}
