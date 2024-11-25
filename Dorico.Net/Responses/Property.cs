namespace DoricoNet.Responses;

/// <summary>
/// Details on an individual property
/// </summary>
/// <param name="Name">The name of the property</param>
/// <param name="DisplayName">A human readable name for the property</param>
/// <param name="ValueType">The type of this property</param>
/// <param name="CurrentValue">The current value of the property if set consistently on the current selection</param>
/// <param name="EnumValues">Values for enums</param>
/// <param name="SetGlobally">Whether this property is set globally</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords",
    Justification = "Name is valid")]
public record Property(
    string Name,
    string? DisplayName,
    string ValueType,
    string? CurrentValue,
    IEnumerable<string>? EnumValues,
    bool? SetGlobally)
{
    /// <inheritdoc/>
    public override string ToString() => string.IsNullOrWhiteSpace(DisplayName) ? "<Property>" : DisplayName;
}
