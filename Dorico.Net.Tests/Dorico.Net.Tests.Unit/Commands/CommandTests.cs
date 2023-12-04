using DoricoNet.Commands;
using DoricoNet.Responses;
using System.Diagnostics.CodeAnalysis;

namespace Dorico.Net.Tests.Commands;

[ExcludeFromCodeCoverage]
[TestFixture]
public class CommandTests
{
    private Command _command;
    private CommandInfo _commandInfo;

    [SetUp]
    public void Setup()
    {
        _commandInfo = new CommandInfo("TestCommand", "Test Command");
        _command = new Command(_commandInfo);
    }

    [Test]
    public void TestCommandCreation()
    {
        var info = (CommandInfo?)_command;

        Assert.Multiple(() =>
        {
            Assert.That(_command.Name, Is.EqualTo(_commandInfo.Name));
            Assert.That(_command.Parameters, Is.Not.Null);
            Assert.That(_command.Parameters, Is.Empty);
            Assert.That(info, Is.Not.Null);
        });
    }

    [Test]
    public void TestCommandCreationWithParamsList()
    {
        var parm1 = new CommandParameter("param1", "value1");
        var parm2 = new CommandParameter("param2", "value2");
        var parm3 = new CommandParameter("param2", "value3");

        _command = new Command(_commandInfo, parm1, parm2);
        _command.AddParameter(parm3);

        var info = (CommandInfo?)_command;

        Assert.Multiple(() =>
        {
            Assert.That(_command.Name, Is.EqualTo(_commandInfo.Name));
            Assert.That(_command.Parameters, Is.Not.Null);
            Assert.That(_command.Parameters, Has.Count.EqualTo(2));
            Assert.That(_command.Parameters[0], Is.EqualTo(parm1));
            Assert.That(_command.Parameters[1], Is.EqualTo(parm3));
            Assert.That(info, Is.Not.Null);
        });
    }

    [Test]
    public void TestCommandCreationByName()
    {
        const string name = "NamedCommand";
        _command = new Command(name);

        var info = (CommandInfo?)_command;

        Assert.Multiple(() =>
        {
            Assert.That(_command.Name, Is.EqualTo(name));
            Assert.That(_command.Parameters, Is.Not.Null);
            Assert.That(_command.Parameters, Is.Empty);
            Assert.That(info, Is.Null);
        });
    }

    [Test]
    public void TestCommandCreationByNameWithParamsList()
    {
        const string name = "NamedCommand";
        var parm1 = new CommandParameter("param1", "value1");
        var parm2 = new CommandParameter("param2", "value2");
        var parm3 = new CommandParameter("param2", "value3");

        _command = new Command(name, parm1, parm2);
        _command.AddParameter(parm3);

        var info = (CommandInfo?)_command;

        Assert.Multiple(() =>
        {
            Assert.That(_command.Name, Is.EqualTo(name));
            Assert.That(_command.Parameters, Is.Not.Null);
            Assert.That(_command.Parameters, Has.Count.EqualTo(2));
            Assert.That(_command.Parameters[0], Is.EqualTo(parm1));
            Assert.That(_command.Parameters[1], Is.EqualTo(parm3));
            Assert.That(info, Is.Null);
        });
    }

    [Test]
    public void TestAddParameter()
    {
        _command.AddParameter(new CommandParameter("param1", "value1"));

        Assert.Multiple(() =>
        {
            Assert.That(_command.Parameters, Has.Count.EqualTo(1));
            Assert.That(_command.Parameters.First().Name, Is.EqualTo("param1"));
            Assert.That(_command.Parameters.First().Value, Is.EqualTo("value1"));
        });
    }

    [Test]
    public void TestAddParameterWhenParameterIsAlreadyPresent()
    {
        // Add a parameter
        _command.AddParameter(new CommandParameter("param1", "value1"));

        // Ensure the parameter was added correctly
        Assert.Multiple(() =>
        {
            Assert.That(_command.Parameters.First().Name, Is.EqualTo("param1"));
            Assert.That(_command.Parameters.First().Value, Is.EqualTo("value1"));
        });

        // Add the same parameter
        _command.AddParameter(new CommandParameter("param1", "value1"));

        // Check that the parameter was updated correctly
        Assert.Multiple(() =>
        {
            Assert.That(_command.Parameters, Has.Count.EqualTo(1));
            Assert.That(_command.Parameters.First().Name, Is.EqualTo("param1"));
            Assert.That(_command.Parameters.First().Value, Is.EqualTo("value1"));
        });
    }

    [Test]
    public void TestAddParameterWhenParameterUpdateExistingParameter()
    {
        // Add a parameter
        _command.AddParameter(new CommandParameter("param1", "value1"));

        // Ensure the parameter was added correctly
        Assert.Multiple(() =>
        {
            Assert.That(_command.Parameters.First().Name, Is.EqualTo("param1"));
            Assert.That(_command.Parameters.First().Value, Is.EqualTo("value1"));
        });

        // Add the same parameter with a different value
        _command.AddParameter(new CommandParameter("param1", "value2"));

        // Check that the parameter was updated correctly
        Assert.Multiple(() =>
        {
            Assert.That(_command.Parameters, Has.Count.EqualTo(1));
            Assert.That(_command.Parameters.First().Name, Is.EqualTo("param1"));
            Assert.That(_command.Parameters.First().Value, Is.EqualTo("value2"));
        });
    }

    [Test]
    public void TestToString()
    {
        _command.AddParameter(new CommandParameter("param1", "value1"));
        var expectedString = "{\"message\": \"command\",\"command\": \"TestCommand?param1=value1\"}";

        Assert.That(_command.ToString(), Is.EqualTo(expectedString));
    }

    [Test]
    public void TestImplicitConversion()
    {
        CommandInfo commandInfo = _command!;
        var commandInfoNull = (CommandInfo?)(Command)null!;
      
        Assert.Multiple(() =>
        {
            Assert.That(commandInfo, Is.EqualTo(_commandInfo));
            Assert.That(commandInfoNull, Is.Null);
        });
    }

    [Test]
    public void TestMessageProperty()
    {
        _command.AddParameter(new CommandParameter("param1", "value1"));
        var expectedMessage = "{\"message\": \"command\",\"command\": \"TestCommand?param1=value1\"}";

        Assert.That(_command.Message, Is.EqualTo(expectedMessage));
    }

    [Test]
    public void TestToCommandInfoMethod()
    {
        var commandInfo = _command.ToCommandInfo()!;

        Assert.Multiple(() =>
        {
            Assert.That(commandInfo.Name, Is.EqualTo(_commandInfo.Name));
            Assert.That(commandInfo.DisplayName, Is.EqualTo(_commandInfo.DisplayName));
        });
    }
}
