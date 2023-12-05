using CommunityToolkit.Diagnostics;
using DoricoNet.Enums;
using DoricoNet.Responses;

namespace DoricoNet.Requests;

/// <summary>
/// Requests information about a specified set of properties.
/// </summary>
public record GetOptionsRequest : DoricoRequestBase<OptionsListResponse>
{
    private readonly string _idString;

    /// <inheritdoc/>
    public override string Message => $"{{\"message\": \"getoptions\", \"optionsType\": \"{OptionsType}\"{_idString}}}";

    /// <inheritdoc/>
    public override string MessageId => "getoptions";

    /// <summary>
    /// The type of options being requested.
    /// </summary>
    public OptionsType OptionsType { get; }

    /// <summary>
    /// GetOptionsRequest constructor.
    /// </summary>
    /// <param name="optionsType">The type of options to request.</param>
    /// <param name="id">A layout ID for OptionType.kLayout, a flow ID for OptionType.kNotation, otherwise null.</param>
    public GetOptionsRequest(OptionsType optionsType, int? id = null)
    {
        OptionsType = optionsType;

        switch (optionsType)
        {
            default:
            case OptionsType.kEngraving:
                _idString = string.Empty;
                break;
            case OptionsType.kLayout:
                Guard.IsNotNull(id, nameof(id));
                _idString = $", \"layoutID\":{id}";
                break;
            case OptionsType.kNotation:
                Guard.IsNotNull(id, nameof(id));
                _idString = $",\"flowID\":{id}";
                break;
        }
    }
}
