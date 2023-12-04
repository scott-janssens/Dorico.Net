using DoricoNet.Attributes;
using Lea;

namespace DoricoNet.Responses;

[ResponseMessage("responseBase")]
public abstract record DoricoResponseBase : IDoricoResponse, IEvent
{
    public required virtual string Message { get; init; }

    public string? RawJson { get; set; }
}
