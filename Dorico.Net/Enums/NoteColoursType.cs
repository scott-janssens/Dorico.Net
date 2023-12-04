using System.Text.Json.Serialization;

namespace DoricoNet.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NoteColoursType
{
    Undefined = 0,
    kNone,
    kColourVoices,
    kColourNotesOutOfRange
}
