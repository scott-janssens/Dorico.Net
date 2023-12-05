using DoricoNet.Responses;

namespace DoricoNet.Exceptions;

/// <summary>
/// Thrown when and error occurs communicating with Dorico.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Not needed")]
public class DoricoException : Exception
{
    /// <summary>
    /// DoricoException constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DoricoException(string message) : base(message)
    {
    }

    /// <summary>
    /// DoricoException constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference.</param>
    public DoricoException(string message, Exception innerException) : base(message, innerException)
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
    /// <summary>
    /// The error Response object associated with the error.
    /// </summary>
    public T Response { get; set; }

    /// <summary>
    /// DoricoException constructor.
    /// </summary>
    /// <param name="response">The error Response object associated with the error.</param>
    /// <param name="message">The message that describes the error.</param>
    public DoricoException(T response, string message) : base(message)
    {
        Response = response;
    }
}
