using DoricoNet.Enums;
using DoricoNet.Responses;

namespace DoricoNet.Requests;

public abstract record SetOptionsRequest : DoricoRequestBase<Response>
{
    protected IEnumerable<int>? Ids { get; }

    public override string Message => $"{{\"message\": \"setoptions\", \"optionsType\": \"{OptionsType}\"{GetIdString()}, \"optionvalues\": [{string.Join(',', OptionValueMessages)}]}}";

    public override string MessageId => "setoptions";

    public OptionsType OptionsType { get; }

    public IEnumerable<string> OptionValueMessages { get; }

    protected SetOptionsRequest(OptionsType optionsType, IEnumerable<OptionValue> optionValues, IEnumerable<int>? ids = null)
    {
        OptionsType = optionsType;
        OptionValueMessages = optionValues.Select(x => x.RequestTemplate);
        Ids = ids;
    }

    protected abstract string GetIdString();
}
