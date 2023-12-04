using DoricoNet.Responses;

namespace DoricoNet.Requests;

public interface IDoricoRequest
{
    string Message { get; }

    string MessageId { get; }

    bool IsAborted { get; }

    Type ResponseType { get; }

    IDoricoResponse? Response { get; }

    Response? ErrorResponse { get; }
}

public interface IDoricoRequest<out TResponse> : IDoricoRequest
{
    TResponse? TypedResponse { get; }
}