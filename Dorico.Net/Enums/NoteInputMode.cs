using System.Text.Json.Serialization;

namespace DoricoNet.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NoteInputMode
{
    Undefined = 0,
    kInsert,
    kOverwrite,
    kChordMerge
}
