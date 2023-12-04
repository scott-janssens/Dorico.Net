using DoricoNet.Responses;

namespace DoricoNet.Requests;

public record GetLibraryEntriesRequest(string Collection) : DoricoRequestBase<LibraryEntitiesListResponse>
{
    public override string Message => $"{{\"message\": \"getlibraryentities\",\"collection\":\"{Collection}\"}}";

    public override string MessageId => "getlibraryentities";
}
