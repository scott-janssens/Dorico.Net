using DoricoNet.Attributes;

namespace DoricoNet.Responses;

/// <summary>
/// Details information about the properties for the current selection
/// </summary>
/// <param name="OpenScoreID">The ID of the score to which the selection belongs</param>
/// <param name="CurrentScope">The scope in which properties will currently be set when given a new value.</param>
/// <param name="EventTypes">Current event types</param>
/// <param name="Properties">The list of properties</param>
[ResponseMessage("propertieslist")]
public record PropertiesListResponse(int OpenScoreID, string CurrentScope, IEnumerable<string>? EventTypes, IEnumerable<Property> Properties) : DoricoResponseBase;
