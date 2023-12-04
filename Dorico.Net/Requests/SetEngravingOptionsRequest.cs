using DoricoNet.Enums;
namespace DoricoNet.Requests;

public record SetEngravingOptionsRequest : SetOptionsRequest
{
    public SetEngravingOptionsRequest(IEnumerable<OptionValue> optionValues)
        : base(OptionsType.kEngraving, optionValues)
    {
    }

    protected override string GetIdString() => string.Empty;
}
