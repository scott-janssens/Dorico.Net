using DoricoNet.DataStructures;
using System.Diagnostics.CodeAnalysis;

namespace Dorico.Net.Tests.DataStructures;

[ExcludeFromCodeCoverage]
[TestFixture]
public class OrganizedTreeListNodeTests
{
    private class TestOrganizable : IOrganizable
    {
        public Func<string> GetNameValue { get; }

        public TestOrganizable(string name)
        {
            GetNameValue = () => name;
        }
    }

    [Test]
    public void TestConstructor()
    {
        var node = new OrganizedTreeListNode<TestOrganizable>("test");
        Assert.Multiple(() =>
        {
            Assert.That(node.Path, Is.EqualTo("test"));
            Assert.That(node.Values, Is.Empty);
            Assert.That(node.ChildNodes, Is.Empty);
        });
    }

    [Test]
    public void TestConstructorWithEnumerable()
    {
        var items = new List<TestOrganizable>
        {
            new("item1"),
            new("item2")
        };

        var node = new OrganizedTreeListNode<TestOrganizable>(items, x => x.GetNameValue());

        Assert.Multiple(() =>
        {
            Assert.That(node.Path, Is.EqualTo("."));
            Assert.That(node.Values, Is.EquivalentTo(items));
            Assert.That(node.ChildNodes, Is.Empty);
        });
    }

    [Test]
    public void TestConstructorWithEnumerableThrows()
    {
        var items = new List<TestOrganizable>
        {
            new("item1"),
            new(null!)
        };

        Assert.Throws<InvalidOperationException>(() => new OrganizedTreeListNode<TestOrganizable>(items, x => x.GetNameValue()));
    }

    [Test]
    public void TestFindParentNode()
    {
        var items = new List<TestOrganizable>
        {
            new("parent1.sub1.item1"),
            new("parent1.sub2.item2"),
            new("parent2.item1")
        };

        var node = new OrganizedTreeListNode<TestOrganizable>(items, x => x.GetNameValue());

        var root = node.FindParentNode("parent1", node);

        Assert.Multiple(() =>
        {
            Assert.That(root.Path, Is.EqualTo("."));
            Assert.That(root.ChildNodes, Has.Exactly(2).Items);
            Assert.That(root.ChildNodes[0].ChildNodes, Has.Exactly(2).Items);
            Assert.That(root.ChildNodes[0].ChildNodes[1].Values[0].GetNameValue(), Is.EqualTo("parent1.sub2.item2"));
            Assert.That(root.ChildNodes[1].Values, Has.Exactly(1).Items);
        });
    }

    [Test]
    public void TestToString()
    {
        var node = new OrganizedTreeListNode<TestOrganizable>("test");
        Assert.That(node.ToString(), Is.EqualTo("test"));
    }
}
