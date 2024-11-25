using CommunityToolkit.Diagnostics;
using System.Collections.Immutable;

namespace DoricoNet.DataStructures;

/// <summary>
/// Class implementation of a tree node in an organized list
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// OrganizedTreeListNode constructor.
/// </remarks>
/// <param name="path">The path of this node from the root node.</param>
public class OrganizedTreeListNode<T>(string path)
    where T : IOrganizable
{
    private readonly List<T> _values = [];

    /// <summary>
    /// The values that have been organized into this node
    /// </summary>
    public ImmutableList<T> Values => [.. _values];

    /// <summary>
    /// The path from the root node to this node.
    /// </summary>
    public string Path { get; init; } = path;

    /// <summary>
    /// A collection of child OrganizedTreeListNodes.
    /// </summary>
    public IList<OrganizedTreeListNode<T>> ChildNodes { get; } = [];

    /// <summary>
    /// OrganizedTreeListNode constructor. The collection passed in organized in a tree structure
    /// with this node becoming the root node.
    /// </summary>
    /// <param name="enumerable">A collection of objects to be organized.</param>
    /// <param name="path"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public OrganizedTreeListNode(IEnumerable<T> enumerable, string path = ".")
        : this(path)
    {
        Guard.IsNotNull(enumerable, nameof(enumerable));

        foreach (var item in enumerable)
        {
            if (item.GetNameValue() is not string propertyValue)
            {
                throw new InvalidOperationException("Unable to organize tree node item because property value is null.");
            }

            var parentNode = FindParentNode(propertyValue, this);
            parentNode._values.Add(item);
        }
    }

    /// <summary>
    /// Returns the parent node of the node with the specified name.
    /// </summary>
    /// <param name="name">The name of a node to search for.</param>
    /// <param name="root">The root node of the organized tree.</param>
    /// <returns></returns>
    public OrganizedTreeListNode<T> FindParentNode(string name, OrganizedTreeListNode<T> root)
    {
        Guard.IsNotNull(name);
        Guard.IsNotNull(root);

        OrganizedTreeListNode<T>? node = null;

        var dotIndex = name.IndexOf('.', StringComparison.Ordinal);

        if (dotIndex != -1)
        {
            var parentName = name[..dotIndex];

            node = root.ChildNodes.FirstOrDefault(x => x.Path == parentName);

            if (node == null)
            {
                node = new OrganizedTreeListNode<T>(parentName);
                root.ChildNodes.Add(node);
            }

            var nextParent = name[(dotIndex + 1)..];
            dotIndex = nextParent.IndexOf('.', StringComparison.Ordinal);

            if (dotIndex != -1)
            {
                node = FindParentNode(nextParent, node);
            }
        }

        return node ?? root;
    }

    /// <inheritdoc/>
    public override string ToString() => Path;
}
