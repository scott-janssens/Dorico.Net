using DoricoNet.Responses;

namespace DoricoNet.Exceptions;

/// <summary>
/// Thrown when and error occurs communicating with Dorico.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Not needed")]
public class DoricoException : Exception
{
    public DoricoException(string message) : base(message)
    {
    }
}

/// <summary>
/// Thrown when Dorico returns an error response.
/// </summary>
/// <typeparam name="T">Type of Dorico response</typeparam>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Not needed")]
public class DoricoException<T> : DoricoException
    where T : DoricoResponseBase
{
    public T Response { get; set; }

    public DoricoException(T response, string message) : base(message)
    {
        Response = response;
    }
}
