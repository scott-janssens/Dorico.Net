using DoricoNet.DataStructures;
using DoricoNet.Responses;
using System.Diagnostics.CodeAnalysis;

namespace Dorico.Net.Tests.DataStructures;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CommandCollectionTests
{
    private CommandCollection _commandList;

    [SetUp]
    public void Setup()
    {
        _commandList = new CommandCollection
            {
                new CommandInfo("Path1.Command1", "Display1", new List<string> { "Param1" }, new List<string> { "OptParam1" }),
                new CommandInfo("Path1.Command2", "Display2", new List<string> { "Param2" }, new List<string> { "OptParam2" }),
                new CommandInfo("Path2.Command1", "Display1", new List<string> { "Param3" }, new List<string> { "OptParam3" })
            };
    }

    [Test]
    public void CommandList_ValidName_ReturnsCommandInfo()
    {
        var commandInfo = _commandList["Path1.Command1"];

        Assert.Multiple(() =>
        {
            Assert.That(commandInfo.Name, Is.EqualTo("Path1.Command1"));
            Assert.That(commandInfo.DisplayName, Is.EqualTo("Display1"));
            Assert.That(commandInfo.RequiredParameters.First(), Is.EqualTo("Param1"));
            Assert.That(commandInfo.OptionalParameters.First(), Is.EqualTo("OptParam1"));
        });
    }

    [Test]
    public void CommandList_InvalidName_ThrowsKeyNotFoundException()
    {
        Assert.Throws<KeyNotFoundException>(() => { var commandInfo = _commandList["InvalidCommand"]; });
    }

    [Test]
    public void CommandList_ReturnsOrganizedTreeListNode()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_commandList.OrganizedItems, Is.Not.Null);
            Assert.That(_commandList.OrganizedItems!.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(_commandList.OrganizedItems!.ChildNodes[0].Path, Is.EqualTo("Path1"));
            Assert.That(_commandList.OrganizedItems!.ChildNodes[0].Values, Has.Count.EqualTo(2));
            Assert.That(_commandList.OrganizedItems!.ChildNodes[1].Path, Is.EqualTo("Path2"));
            Assert.That(_commandList.OrganizedItems!.ChildNodes[1].Values, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void CommandList_TryGetValue()
    {
        var isItem1Found = _commandList.TryGetValue("Path1.Command1", out var item1);
        var isItem2Found = _commandList.TryGetValue("Nope", out var item2);

        Assert.Multiple(() =>
        {
            Assert.That(isItem1Found, Is.True);
            Assert.That(isItem2Found, Is.False);
            Assert.That(item1, Is.Not.Null);
            Assert.That(item2, Is.Null);
        });
    }
}
