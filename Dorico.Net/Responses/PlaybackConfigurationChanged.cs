using DoricoNet.Attributes;

namespace DoricoNet.Responses;

[ResponseMessage("playbackconfigurationchanged")]
public record PlaybackConfigurationChanged(int OpenScoreId) : DoricoUnpromptedResponseBase;
