namespace DoricoNet.Responses;

/// <summary>
/// Details on an expression map
/// </summary>
/// <param name="ExpressionMapId">The ID of the expression map</param>
/// <param name="ExpressionMapEntries">The list of techniques in the expression map</param>
public record ExpressionMap(string ExpressionMapId, IEnumerable<ExpressionMapEntry> ExpressionMapEntries);
