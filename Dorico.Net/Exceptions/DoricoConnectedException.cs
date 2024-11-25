namespace DoricoNet.Exceptions;

/// <summary>
/// Thrown when an operation expects an open connection to Dorico, but there is none.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors",
    Justification = "Not needed")]
public class DoricoConnectedException : Exception
{
    /// <summary>
    /// DoricoConnectedException constructor.
    /// </summary>
    public DoricoConnectedException() : base("DoricoRemote is already connected to Dorico.")
    {
    }
}
