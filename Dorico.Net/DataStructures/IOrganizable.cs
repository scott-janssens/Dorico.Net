namespace DoricoNet.DataStructures;

/// <summary>
/// Interface for organizable lists
/// </summary>
public interface IOrganizable
{
    /// <summary>
    /// Returns a delegate that returns the value used to organize the type in a OrganizedList
    /// </summary>
    Func<string> GetNameValue { get; }
}
