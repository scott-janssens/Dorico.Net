namespace DoricoNet.Exceptions;

/// <summary>
/// Thrown when an operation expects no connection to Dorico, but there one is open.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Not needed")]
public class DoricoNotConnectedException : Exception
{
    /// <summary>
    /// DoricoNotConnectedException constructor.
    /// </summary>
    public DoricoNotConnectedException() : base("DoricoRemote is not connected to Dorico.")
    {
    }
}
