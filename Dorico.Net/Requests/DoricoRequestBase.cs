using CommunityToolkit.Diagnostics;
using DoricoNet.Responses;

namespace DoricoNet.Requests;

public abstract record DoricoRequestBase : IDoricoRequest
{
    /// <summary>
    /// Message texts sent to Dorico.
    /// </summary>
    public abstract string Message { get; }

    public abstract string MessageId { get; }

    public bool IsAborted { get; protected set; }

    public abstract Type ResponseType { get; }

    public virtual IDoricoResponse? Response { get; protected set; }

    public Response? ErrorResponse { get; protected set; }

    internal virtual void SetResponse(IDoricoResponse response) => Response = response;

    internal virtual void SetErrorResponse(Response response) => ErrorResponse = response;

    internal virtual void Abort() => IsAborted = true;
}

public abstract record DoricoRequestBase<TResponse> : DoricoRequestBase, IDoricoRequest<TResponse>
    where TResponse : DoricoResponseBase
{
    public override IDoricoResponse? Response => TypedResponse;

    public TResponse? TypedResponse { get; private set; }

    public override Type ResponseType => typeof(TResponse);

    internal override void SetResponse(IDoricoResponse? response)
    {
        Response = response;
        TypedResponse = (TResponse?)response;
    }
}
