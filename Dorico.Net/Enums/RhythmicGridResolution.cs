using DoricoNet.Json;
using System.Text.Json.Serialization;

namespace DoricoNet.Enums;

[JsonConverter(typeof(RhythmicGridResolutionConverter))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1069:Enums values should not be duplicated",
    Justification = "Deliberate duplication")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1027:Mark enums with FlagsAttribute",
    Justification = "Not flags")]
public enum RhythmicGridResolution
{
    Undefined = 0,
    kSemibreve = 1,
    Whole = 1,
    kDottedMinim = 2,
    DottedHalf = 2,
    kMinim = 3,
    Half = 3,
    kDottedCrotchet = 4,
    DottedQuarter = 4,
    kCrotchet = 5,
    Quarter = 5,
    kDottedQuaver = 6,
    DottedEighth = 6,
    kQuaver = 7,
    Eighth = 7,
    kDottedSemiquaver = 8,
    DottedSixteenth = 8,
    kSemiquaver = 9,
    Sixteenth = 9,
    kDemisemiquaver = 10,
    ThirtySecond = 10
}
