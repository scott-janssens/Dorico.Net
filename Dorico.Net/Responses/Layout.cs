using DoricoNet.Enums;
using System.Diagnostics.CodeAnalysis;

namespace DoricoNet.Responses;

/// <summary>
/// Details on an individual layout
/// </summary>
/// <param name="LayoutID">The ID of this layout</param>
/// <param name="LayoutName">The name of this layout</param>
/// <param name="LayoutNumber">The number of this layout</param>
/// <param name="LayoutType">The type of this layout. Permitted value: kFullScoreLayout, kPartLayout,
/// kCustomScoreLayout</param>
[SuppressMessage("Naming", "CA1724:Type names should not match namespaces", Justification = "Name is fine")]
public record Layout(int LayoutID, string LayoutName, int LayoutNumber, LayoutType LayoutType)
{
    /// <inheritdoc/>
    public override string ToString() => LayoutName;
}
