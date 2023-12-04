using DoricoNet.Responses;

namespace DoricoNet.Requests;

public record GetPlaybackTechniquesRequest : DoricoRequestBase<PlaybackTechniquesListResponse>
{
    public override string Message => "{ \"message\": \"getplaybacktechniques\"}";

    public override string MessageId => "getplaybacktechniques";
}
