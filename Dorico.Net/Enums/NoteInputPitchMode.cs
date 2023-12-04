using System.Text.Json.Serialization;

namespace DoricoNet.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NoteInputPitchMode
{
    Undefined = 0,
    kWrittenPitch,
    kSoundingPitch
}
