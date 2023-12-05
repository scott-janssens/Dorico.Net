using DoricoNet.Enums;
using DoricoNet.Responses;

namespace DoricoNet.Requests;

/// <summary>
/// Instructs Dorico to set the specified option values.
/// </summary>
public abstract record SetOptionsRequest : DoricoRequestBase<Response>
{
    private readonly IEnumerable<string> _optionValueMessages;

    protected IEnumerable<int>? Ids { get; }

    /// <inheritdoc/>
    public override string Message => $"{{\"message\": \"setoptions\", \"optionsType\": \"{OptionsType}\"{GetIdString()}, \"optionvalues\": [{string.Join(',', _optionValueMessages)}]}}";

    /// <inheritdoc/>
    public override string MessageId => "setoptions";

    /// <summary>
    /// The type of options to set.
    /// </summary>
    public OptionsType OptionsType { get; }

    /// <summary>
    /// SetOptionsRequest constructor.
    /// </summary>
    /// <param name="optionsType">The type of options to set.</param>
    /// <param name="optionValues">The list of options and values to set.</param>
    /// <param name="ids">A collection of layout IDs for OptionType.kLayout, flow IDs for OptionType.kNotation, 
    /// otherwise null.</param>
    protected SetOptionsRequest(OptionsType optionsType, IEnumerable<OptionValue> optionValues, IEnumerable<int>? ids = null)
    {
        OptionsType = optionsType;
        _optionValueMessages = optionValues.Select(x => x.RequestTemplate);
        Ids = ids;
    }

    protected abstract string GetIdString();
}
