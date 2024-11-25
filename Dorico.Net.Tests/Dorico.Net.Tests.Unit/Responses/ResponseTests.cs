using DoricoNet.Enums;
using DoricoNet.Responses;
using System.Diagnostics.CodeAnalysis;

namespace Dorico.Net.Tests.Unit.Responses;

[ExcludeFromCodeCoverage]
[TestFixture]
public class ResponseTests
{
    [Test]
    public void CommandInfoToString()
    {
        var response = new CommandInfo("TestName", "TestDisplayName");
        Assert.That(response.ToString(), Is.EqualTo("TestDisplayName"));

        response = new CommandInfo("TestName");
        Assert.That(response.ToString(), Is.EqualTo("TestName"));
    }

    [Test]
    public void DrumKitNoteMapEntryToString()
    {
        var response = new DrumKitNoteMapEntry("TestName", "", 0, null, null);
        Assert.That(response.ToString(), Is.EqualTo("TestName"));

        response = new DrumKitNoteMapEntry(" ", "", 0, null, null);
        Assert.That(response.ToString(), Is.EqualTo("<DrumKitNoteMapEntry>"));

        response = new DrumKitNoteMapEntry(null, "", 0, null, null);
        Assert.That(response.ToString(), Is.EqualTo("<DrumKitNoteMapEntry>"));
    }

    [Test]
    public void ExpressionMapEntryToString()
    {
        var response = new ExpressionMapEntry("TestName", [""]);
        Assert.That(response.ToString(), Is.EqualTo("TestName"));
    }

    [Test]
    public void FlowToString()
    {
        var response = new Flow(1, "TestName");
        Assert.That(response.ToString(), Is.EqualTo("TestName"));
    }

    [Test]
    public void LayoutToString()
    {
        var response = new Layout(1, "TestName", 1, LayoutType.kAll);
        Assert.That(response.ToString(), Is.EqualTo("TestName"));
    }

    [Test]
    public void PropertyToString()
    {
        var response = new Property("TestName", "DisplayName", "", "", null, false);
        Assert.That(response.ToString(), Is.EqualTo("DisplayName"));

        response = new Property("TestName", "", "", "", null, false);
        Assert.That(response.ToString(), Is.EqualTo("<Property>"));

        response = new Property("TestName", null, "", "", null, false);
        Assert.That(response.ToString(), Is.EqualTo("<Property>"));
    }

    [Test]
    public void ResponseToString()
    {
        var response = new Response("Code", "Detail") { Message = "response" };
        Assert.That(response.ToString(), Is.EqualTo("Response: Code Detail"));
    }

    [Test]
    public void VersionToString()
    {
        var response = new VersionResponse("Variant", "Number") { Message = "version" };
        Assert.That(response.ToString(), Is.EqualTo("Variant Number"));
    }
}