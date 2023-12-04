using DoricoNet.Enums;

namespace DoricoNet.Requests;

public record SetNotationOptionsRequest : SetOptionsRequest
{
    public FlowIds? FlowIds { get; }

    public SetNotationOptionsRequest(IEnumerable<OptionValue> optionValues, IEnumerable<int>? flowIds = null)
        : base(OptionsType.kNotation, optionValues, flowIds)
    {
    }

    public SetNotationOptionsRequest(IEnumerable<OptionValue> optionValues, FlowIds flowIds)
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
