using DoricoNet.Responses;

namespace DoricoNet.Requests;

public record GetLibraryCollectionsRequest : DoricoRequestBase<LibraryCollectionsListResponse>
{
    public override string Message => "{\"message\": \"getlibrarycollections\"}";

    public override string MessageId => "getlibrarycollections";
}
