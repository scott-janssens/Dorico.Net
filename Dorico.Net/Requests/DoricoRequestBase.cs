using DoricoNet.Responses;

namespace DoricoNet.Requests;

public abstract record DoricoRequestBase : IDoricoRequest
{
    /// <inheritdoc/>
    public abstract string Message { get; }

    /// <inheritdoc/>
    public abstract string MessageId { get; }

    /// <inheritdoc/>
    public bool IsAborted { get; protected set; }

    /// <inheritdoc/>
    public abstract Type ResponseType { get; }

    /// <inheritdoc/>
    public virtual IDoricoResponse? Response { get; protected set; }

    /// <inheritdoc/>
    public Response? ErrorResponse { get; protected set; }

    internal virtual void SetResponse(IDoricoResponse response) => Response = response;

    internal virtual void SetErrorResponse(Response response) => ErrorResponse = response;

    internal virtual void Abort() => IsAborted = true;
}

public abstract record DoricoRequestBase<TResponse> : DoricoRequestBase, IDoricoRequest<TResponse>
    where TResponse : DoricoResponseBase
{
    /// <inheritdoc/>
    public override IDoricoResponse? Response => TypedResponse;

    /// <inheritdoc/>
    public TResponse? TypedResponse { get; private set; }

    /// <inheritdoc/>
    public override Type ResponseType => typeof(TResponse);

    internal override void SetResponse(IDoricoResponse? response)
    {
        Response = response;
        TypedResponse = (TResponse?)response;
    }
}
