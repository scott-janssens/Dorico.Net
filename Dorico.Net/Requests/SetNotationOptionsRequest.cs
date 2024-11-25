using DoricoNet.Enums;

namespace DoricoNet.Requests;

/// <summary>
/// Instructs Dorico to set the specified engraving option values.
/// </summary>
public record SetNotationOptionsRequest : SetOptionsRequest
{
    /// <summary>
    /// A FlowIds enum value on which to set the option values.
    /// </summary>
    public FlowId? FlowIds { get; }

    /// <summary>
    /// SetNotationOptionsRequest constructor.
    /// </summary>
    /// <param name="optionValues">The list of options and values to set.</param>
    /// <param name="flowIds">A collection of flow ID values on which to set the option values.</param>
    public SetNotationOptionsRequest(IEnumerable<OptionValue> optionValues, IEnumerable<int>? flowIds)
        : base(OptionsType.kNotation, optionValues, flowIds)
    {
    }

    /// <summary>
    /// SetNotationOptionsRequest constructor.
    /// </summary>
    /// <param name="optionValues">The list of options and values to set.</param>
    /// <param name="flowIds">A FlowId enum value on which to set the option values.</param>
    public SetNotationOptionsRequest(IEnumerable<OptionValue> optionValues, FlowId flowIds)
        : base(OptionsType.kNotation, optionValues)
    {
        FlowIds = flowIds;
    }

    protected override string GetIdString()
    {
        if (FlowIds != null)
        {
            return $", \"flowIDs\": \"{FlowIds}\"";
        }

        return Ids != null
            ? $", \"flowIDs\": [{string.Join(", ", Ids.Select(x => $"\"{x}\""))}]"
            : string.Empty;
    }
}
