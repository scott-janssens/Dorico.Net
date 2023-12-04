using DoricoNet.Enums;

namespace DoricoNet.Requests;

public record SetLayoutOptionsRequest : SetOptionsRequest
{
    public LayoutIds? LayoutIds { get; }

    public SetLayoutOptionsRequest(IEnumerable<OptionValue> optionValues, IEnumerable<int>? layoutIds = null)
        : base(OptionsType.kLayout, optionValues, layoutIds)
    {
    }

    public SetLayoutOptionsRequest(IEnumerable<OptionValue> optionValues, LayoutIds layoutIds)
        : base(OptionsType.kLayout, optionValues)
    {
        LayoutIds = layoutIds;
    }

    protected override string GetIdString()
    {
        if (LayoutIds != null)
        {
            return $", \"layoutIDs\": \"{LayoutIds}\"";
        }

        return Ids != null
            ? $", \"layoutIDs\": [{string.Join(", ", Ids.Select(x => $"\"{x}\""))}]"
            : string.Empty;
    }
}
