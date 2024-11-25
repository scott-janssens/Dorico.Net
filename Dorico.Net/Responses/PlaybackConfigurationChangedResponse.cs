using DoricoNet.Attributes;

namespace DoricoNet.Responses;

[ResponseMessage("playbackconfigurationchanged")]
public record PlaybackConfigurationChangedResponse(int OpenScoreId) : DoricoUnpromptedResponseBase;
