using DoricoNet.Attributes;
using DoricoNet.DataStructures;

namespace DoricoNet.Responses;

/// <summary>
/// A list of commands which can be sent to Dorico via a commands message
/// </summary>
/// <param name="Commands">The list of commands</param>
[ResponseMessage("commandlist")]
public record CommandListResponse(CommandCollection Commands) : DoricoResponseBase;
