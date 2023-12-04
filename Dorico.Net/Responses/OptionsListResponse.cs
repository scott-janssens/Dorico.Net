using DoricoNet.Attributes;
using DoricoNet.DataStructures;
using DoricoNet.Enums;

namespace DoricoNet.Responses;

/// <summary>
/// Details information about the requested properties
/// </summary>
/// <param name="OpenScoreID">The ID of the score to which these options belong</param>
/// <param name="OptionsType">The type of options being listed.</param>
/// <param name="Options">The list of options, their types and current values</param>
[ResponseMessage("optionslist")]
public record OptionsListResponse(int OpenScoreID, OptionsType OptionsType, OptionCollection Options) : DoricoResponseBase;
