using DoricoNet.Responses;

namespace DoricoNet.Requests;

/// <summary>
/// Requests information about the properties which can be set on the current selection
/// </summary>
public record GetPropertiesRequest : DoricoRequestBase<PropertiesListResponse>
{
    /// <inheritdoc/>
    public override string Message => "{\"message\": \"getproperties\"}";

    /// <inheritdoc/>
    public override string MessageId => "getproperties";
}
