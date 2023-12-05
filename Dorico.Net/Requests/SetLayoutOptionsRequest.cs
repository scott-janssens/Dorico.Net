using DoricoNet.Enums;

namespace DoricoNet.Requests;

/// <summary>
/// Instructs Dorico to set the specified engraving option values.
/// </summary>
public record SetLayoutOptionsRequest : SetOptionsRequest
{
    /// <summary>
    /// A LayoutIDs enum value on which to set the option values.
    /// </summary>
    public LayoutIds? LayoutIds { get; }

    /// <summary>
    /// SetLayoutOptionsRequest constructor
    /// </summary>
    /// <param name="optionValues">The list of options and values to set.</param>
    /// <param name="layoutIds">A collection of layout ID values on which to set the option values.</param>
    public SetLayoutOptionsRequest(IEnumerable<OptionValue> optionValues, IEnumerable<int>? layoutIds)
        : base(OptionsType.kLayout, optionValues, layoutIds)
    {
    }

    /// <summary>
    /// SetLayoutOptionsRequest constructor
    /// </summary>
    /// <param name="optionValues">The list of options and values to set.</param>
    /// <param name="layoutIds">A LayoutIDs enum value on which to set the option values.</param>
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
