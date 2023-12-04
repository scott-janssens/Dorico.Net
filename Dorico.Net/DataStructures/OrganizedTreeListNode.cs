using CommunityToolkit.Diagnostics;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace DoricoNet.DataStructures;

/// <summary>
/// Class implementation of a tree node in an organized list
/// </summary>
/// <typeparam name="T"></typeparam>
public class OrganizedTreeListNode<T>
    where T : IOrganizable
{
    private readonly List<T> _values = new();

    /// <summary>
    /// The values that have been organized into this node
    /// </summary>
    public ImmutableList<T> Values => _values.ToImmutableList();

    public string Path { get; init; }

    public IList<OrganizedTreeListNode<T>> ChildNodes { get; } = new List<OrganizedTreeListNode<T>>();

    public OrganizedTreeListNode(string path)
    {
        Path = path;
    }

    public OrganizedTreeListNode(IEnumerable<T> enumerable, Expression<Func<T, string>> lambda, string path = ".")
        : this(path)
    {
        Guard.IsNotNull(enumerable, nameof(enumerable));
        Guard.IsNotNull(lambda, nameof(lambda));

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

    public override string ToString() => Path;
}
