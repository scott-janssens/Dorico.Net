using DoricoNet.Json;
using System.Text.Json.Serialization;

namespace DoricoNet.Enums;

[JsonConverter(typeof(AccidentalResolutionConverter))]
public enum Accidental
{
    None = 0,
    kNatural,
    kSharp,
    kDoubleSharp,
    kTripleSharp,
    kFlat,
    kDoubleFlat,
    kTripleFlat
}
