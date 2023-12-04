using CommunityToolkit.Diagnostics;
using DoricoNet.Enums;
using DoricoNet.Responses;

namespace DoricoNet.Requests;

public record GetOptionsRequest : DoricoRequestBase<OptionsListResponse>
{
    private readonly string _idString;

    public override string Message => $"{{\"message\": \"getoptions\", \"optionsType\": \"{OptionsType}\"{_idString}}}";

    public override string MessageId => "getoptions";

    public OptionsType OptionsType { get; }

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
