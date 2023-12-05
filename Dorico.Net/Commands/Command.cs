using CommunityToolkit.Diagnostics;
using DoricoNet.Requests;
using DoricoNet.Responses;
using System.Collections.Immutable;

namespace DoricoNet.Commands;

/// <summary>
/// Request object that instructs Dorico to execute a command
/// </summary>
public record Command : DoricoRequestBase<Response>
{
    private readonly CommandInfo? _commandInfo;

    /// <summary>
    /// The name of the command.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// A list of specified parameter values for this command instance.
    /// </summary>
    public ImmutableList<CommandParameter> Parameters { get; private set; }

    /// <inheritdoc/>
    public override string Message => ToString();

    public override string MessageId => "command";

    /// <summary>
    /// Command record constructor
    /// </summary>
    /// <param name="commandInfo">A CommandInfo record describing the command.</param>
    /// <param name="parameters">A collection of CommandParameter objects.</param>
    public Command(CommandInfo commandInfo, params CommandParameter[] parameters)
    {
        Guard.IsNotNull(commandInfo, nameof(commandInfo));

        _commandInfo = commandInfo;
        Name = commandInfo.Name;
        Parameters = ImmutableList.Create<CommandParameter>();

        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                AddParameter(parameter);
            }
        }
    }

    /// <summary>
    /// Command record constructor
    /// </summary>
    /// <param name="name">The name of a Dorico command.</param>
    /// <param name="parameters">A collection of CommandParameter objects.</param>
    public Command(string name, params CommandParameter[] parameters)
    {
        Name = name;
        Parameters = ImmutableList.Create<CommandParameter>();

        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                AddParameter(parameter);
            }
        }
    }

    /// <summary>
    /// Adds parameter information to the Parameters list.
    /// </summary>
    /// <param name="parameter">A CommandParameter object</param>
    /// <param name="value">the value of the parameter</param>
    public void AddParameter(CommandParameter parameter)
    {
        Guard.IsNotNull(parameter, nameof(parameter));

        var existing = Parameters.FirstOrDefault(x => x.Name == parameter.Name);

        if (existing != null)
        {
            if (existing.Value != parameter.Value)
            {
                Parameters = Parameters.Replace(existing, parameter);
            }
        }
        else
        {
            Parameters = Parameters.Add(parameter);
        }
    }

    /// <summary>
    /// Returns the CommandInfo object, if any, used to create this Command object, otherwise null.
    /// </summary>
    /// <returns>A CommandInfo object</returns>
    public CommandInfo? ToCommandInfo() => _commandInfo;

    /// <inheritdoc/>
    public override string ToString()
    {
        var parameters = string.Join(',', Parameters.Select(x => x.ToString()));
        return $"{{\"message\": \"command\",\"command\": \"{Name}?{parameters}\"}}";
    }

    /// <summary>
    /// Returns the CommandInfo object, if any, used to create this Command object, otherwise null.
    /// </summary>
    /// <returns>A CommandInfo object</returns>
    public static implicit operator CommandInfo?(Command command)
    {
        return command?._commandInfo;
    }
}
