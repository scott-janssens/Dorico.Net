using DoricoNet.DataStructures;

namespace DoricoNet.Responses;

/// <summary>
/// Details on an individual property
/// </summary>
/// <param name="Path"><The path to this option/param>
/// <param name="ValueType">The type of this property</param>
/// <param name="CurrentValue">The current value of the option</param>
/// <param name="EnumValues">Values for enums</param>
public sealed record OptionInfo(string Path, string ValueType, string CurrentValue, IEnumerable<string>? EnumValues) : IOrganizable
{
    /// <inheritdoc/>
    Func<string> IOrganizable.GetNameValue => () => Path;
}
