using DoricoNet.Commands;
using System.Diagnostics.CodeAnalysis;

namespace Dorico.Net.Tests.Commands;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CommandParameterTests
{
    [Test]
    public void Ctor()
    {
        var actual = new CommandParameter("Test", "Value");

        Assert.Multiple(() =>
        {
            Assert.That(actual.Name, Is.EqualTo("Test"));
            Assert.That(actual.Value, Is.EqualTo("Value"));
        });
    }

    [Test]
    public void CommandParameterToString()
    {
        var actual = new CommandParameter("Test", "Value");
        Assert.That(actual.ToString(), Is.EqualTo("Test=Value"));
    }
}
