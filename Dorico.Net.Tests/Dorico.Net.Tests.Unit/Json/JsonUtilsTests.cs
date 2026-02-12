using DoricoNet.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Dorico.Net.Tests.Unit.Json;

[ExcludeFromCodeCoverage]
[TestFixture]
internal class JsonUtilsTests
{
    private static JsonElement Parse(string json) =>
        JsonDocument.Parse(json).RootElement;

    [Test]
    public void Merge_DisjointProperties_ContainsBoth()
    {
        var original = """{"a": 1}""";
        var newContent = """{"b": 2}""";

        var result = Parse(JsonUtils.Merge(original, newContent));

        Assert.That(result.GetProperty("a").GetInt32(), Is.EqualTo(1));
        Assert.That(result.GetProperty("b").GetInt32(), Is.EqualTo(2));
    }

    [Test]
    public void Merge_OverlappingProperty_NewContentWins()
    {
        var original = """{"a": 1, "b": 2}""";
        var newContent = """{"b": 99}""";

        var result = Parse(JsonUtils.Merge(original, newContent));

        Assert.That(result.GetProperty("a").GetInt32(), Is.EqualTo(1));
        Assert.That(result.GetProperty("b").GetInt32(), Is.EqualTo(99));
    }

    [Test]
    public void Merge_NewContentNullForExistingProperty_OriginalValuePreserved()
    {
        var original = """{"a": 1, "b": 2}""";
        var newContent = """{"b": null}""";

        var result = Parse(JsonUtils.Merge(original, newContent));

        Assert.That(result.GetProperty("a").GetInt32(), Is.EqualTo(1));
        Assert.That(result.GetProperty("b").GetInt32(), Is.EqualTo(2));
    }

    [Test]
    public void Merge_NewContentNullForUniqueProperty_NullIsWritten()
    {
        var original = """{"a": 1}""";
        var newContent = """{"c": null}""";

        var result = Parse(JsonUtils.Merge(original, newContent));

        Assert.That(result.GetProperty("a").GetInt32(), Is.EqualTo(1));
        Assert.That(result.GetProperty("c").ValueKind, Is.EqualTo(JsonValueKind.Null));
    }

    [Test]
    public void Merge_BothEmpty_ReturnsEmptyObject()
    {
        var result = Parse(JsonUtils.Merge("{}", "{}"));

        Assert.That(result.EnumerateObject().Any(), Is.False);
    }

    [Test]
    public void Merge_OriginalEmptyNewContentHasProperties_ReturnsNewContent()
    {
        var original = "{}";
        var newContent = """{"x": 10}""";

        var result = Parse(JsonUtils.Merge(original, newContent));

        Assert.That(result.GetProperty("x").GetInt32(), Is.EqualTo(10));
    }

    [Test]
    public void Merge_NewContentEmpty_ReturnsOriginal()
    {
        var original = """{"x": 10}""";
        var newContent = "{}";

        var result = Parse(JsonUtils.Merge(original, newContent));

        Assert.That(result.GetProperty("x").GetInt32(), Is.EqualTo(10));
    }

    [Test]
    public void Merge_StringValues_MergedCorrectly()
    {
        var original = """{"name": "Alice"}""";
        var newContent = """{"name": "Bob"}""";

        var result = Parse(JsonUtils.Merge(original, newContent));

        Assert.That(result.GetProperty("name").GetString(), Is.EqualTo("Bob"));
    }

    [Test]
    public void Merge_NestedObjects_NewContentReplacesEntireNestedObject()
    {
        var original = """{"a": {"x": 1, "y": 2}}""";
        var newContent = """{"a": {"x": 99}}""";

        var result = Parse(JsonUtils.Merge(original, newContent));

        var nested = result.GetProperty("a");
        Assert.That(nested.GetProperty("x").GetInt32(), Is.EqualTo(99));
        Assert.That(nested.TryGetProperty("y", out _), Is.False);
    }

    [Test]
    public void Merge_ArrayValues_NewContentReplacesArray()
    {
        var original = """{"items": [1, 2, 3]}""";
        var newContent = """{"items": [4, 5]}""";

        var result = Parse(JsonUtils.Merge(original, newContent));

        var items = result.GetProperty("items").EnumerateArray().Select(e => e.GetInt32()).ToArray();
        Assert.That(items, Is.EqualTo(new[] { 4, 5 }));
    }

    [Test]
    public void Merge_CamelCaseTrue_OriginalPascalCaseMatchesNewCamelCase()
    {
        var original = """{"MyProp": 1}""";
        var newContent = """{"myProp": 42}""";

        var result = Parse(JsonUtils.Merge(original, newContent, camelCase: true));

        Assert.That(result.GetProperty("myProp").GetInt32(), Is.EqualTo(42));
    }

    [Test]
    public void Merge_CamelCaseFalse_PascalAndCamelAreTreatedAsDistinct()
    {
        var original = """{"MyProp": 1}""";
        var newContent = """{"myProp": 42}""";

        var result = Parse(JsonUtils.Merge(original, newContent, camelCase: false));

        Assert.That(result.GetProperty("MyProp").GetInt32(), Is.EqualTo(1));
        Assert.That(result.GetProperty("myProp").GetInt32(), Is.EqualTo(42));
    }

    [Test]
    public void Merge_CamelCaseTrue_NullInNewContentPreservesOriginal()
    {
        var original = """{"MyProp": 5}""";
        var newContent = """{"myProp": null}""";

        var result = Parse(JsonUtils.Merge(original, newContent, camelCase: true));

        Assert.That(result.GetProperty("MyProp").GetInt32(), Is.EqualTo(5));
    }

    [Test]
    public void Merge_MultipleOverlappingAndDisjoint_AllHandledCorrectly()
    {
        var original = """{"a": 1, "b": 2, "c": 3}""";
        var newContent = """{"b": 20, "d": 40}""";

        var result = Parse(JsonUtils.Merge(original, newContent));

        Assert.That(result.GetProperty("a").GetInt32(), Is.EqualTo(1));
        Assert.That(result.GetProperty("b").GetInt32(), Is.EqualTo(20));
        Assert.That(result.GetProperty("c").GetInt32(), Is.EqualTo(3));
        Assert.That(result.GetProperty("d").GetInt32(), Is.EqualTo(40));
    }

    [Test]
    public void Merge_BooleanValues_MergedCorrectly()
    {
        var original = """{"active": true}""";
        var newContent = """{"active": false}""";

        var result = Parse(JsonUtils.Merge(original, newContent));

        Assert.That(result.GetProperty("active").GetBoolean(), Is.False);
    }

    [Test]
    public void Merge_AllOriginalPropertiesNulledOut_OriginalValuesPreserved()
    {
        var original = """{"a": 1, "b": 2}""";
        var newContent = """{"a": null, "b": null}""";

        var result = Parse(JsonUtils.Merge(original, newContent));

        Assert.That(result.GetProperty("a").GetInt32(), Is.EqualTo(1));
        Assert.That(result.GetProperty("b").GetInt32(), Is.EqualTo(2));
    }
}
