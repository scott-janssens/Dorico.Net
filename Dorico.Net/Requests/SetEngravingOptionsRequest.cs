using DoricoNet.Enums;
namespace DoricoNet.Requests;

/// <summary>
/// Instructs Dorico to set the specified engraving option values.
/// </summary>
public record SetEngravingOptionsRequest : SetOptionsRequest
{
    /// <summary>
    /// SetEngravingOptionsRequest constructor
    /// </summary>
    /// <param name="optionValues">The list of options and values to set.</param>
    public SetEngravingOptionsRequest(IEnumerable<OptionValue> optionValues)
        : base(OptionsType.kEngraving, optionValues)
    {
    }

    protected override string GetIdString() => string.Empty;
}
