using DoricoNet.Attributes;

namespace DoricoNet.Responses;

/// <summary>
/// A message containing version information about an instance of Dorico
/// </summary>
/// <param name="Variant">The variant of Dorico</param>
/// <param name="Number">The version number</param>
[ResponseMessage("version")]
public record VersionResponse(string Variant, string Number) : DoricoResponseBase
{
    public override string ToString() => $"{Variant} {Number}";
}
