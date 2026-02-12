using DoricoNet.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dorico.Net.Tests.Unit.Json;

[ExcludeFromCodeCoverage]
[TestFixture]
internal class SafeEnumJsonConverterFactoryTests
{
    private JsonSerializerOptions _options = null!;

    private enum Color { Red, Green, Blue }

    [Flags]
    private enum Permissions { None = 0, Read = 1, Write = 2, Execute = 4 }

    private class NonNullableModel
    {
        public Color Color { get; set; }
    }

    private class NullableModel
    {
        public Color? Color { get; set; }
    }

    [SetUp]
    public void Setup()
    {
        SafeEnumJsonConverterFactory.UnknownEnumStringToken = null;
        SafeEnumJsonConverterFactory.UnknownEnumNumberToken = null;

        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new SafeEnumJsonConverterFactory() }
        };
    }

    // --- CanConvert ---

    [Test]
    public void CanConvert_EnumType_ReturnsTrue()
    {
        var factory = new SafeEnumJsonConverterFactory();
        Assert.That(factory.CanConvert(typeof(Color)), Is.True);
    }

    [Test]
    public void CanConvert_NullableEnumType_ReturnsTrue()
    {
        var factory = new SafeEnumJsonConverterFactory();
        Assert.That(factory.CanConvert(typeof(Color?)), Is.True);
    }

    [Test]
    public void CanConvert_NonEnumType_ReturnsFalse()
    {
        var factory = new SafeEnumJsonConverterFactory();
        Assert.That(factory.CanConvert(typeof(int)), Is.False);
    }

    [Test]
    public void CanConvert_NullType_ThrowsArgumentNullException()
    {
        var factory = new SafeEnumJsonConverterFactory();
        Assert.Throws<ArgumentNullException>(() => factory.CanConvert(null!));
    }

    // --- Non-nullable string deserialization ---

    [Test]
    public void Read_StringToken_ParsesEnumValue()
    {
        var result = JsonSerializer.Deserialize<NonNullableModel>("{\"Color\":\"Green\"}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(Color.Green));
    }

    [Test]
    public void Read_StringToken_CaseInsensitive()
    {
        var result = JsonSerializer.Deserialize<NonNullableModel>("{\"Color\":\"bLuE\"}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(Color.Blue));
    }

    [Test]
    public void Read_StringToken_EmptyString_ReturnsDefault()
    {
        var result = JsonSerializer.Deserialize<NonNullableModel>("{\"Color\":\"\"}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(default(Color)));
    }

    [Test]
    public void Read_StringToken_Whitespace_ReturnsDefault()
    {
        var result = JsonSerializer.Deserialize<NonNullableModel>("{\"Color\":\"  \"}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(default(Color)));
    }

    [Test]
    public void Read_StringToken_UnknownValue_ReturnsDefault_And_InvokesCallback()
    {
        Type? reportedType = null;
        string? reportedValue = null;
        SafeEnumJsonConverterFactory.UnknownEnumStringToken = (t, v) => { reportedType = t; reportedValue = v; };

        var result = JsonSerializer.Deserialize<NonNullableModel>("{\"Color\":\"Yellow\"}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(default(Color)));
        Assert.That(reportedType, Is.EqualTo(typeof(Color)));
        Assert.That(reportedValue, Is.EqualTo("Yellow"));
    }

    // --- Non-nullable number deserialization ---

    [Test]
    public void Read_NumberToken_DefinedValue_ParsesEnumValue()
    {
        var result = JsonSerializer.Deserialize<NonNullableModel>("{\"Color\":2}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(Color.Blue));
    }

    [Test]
    public void Read_NumberToken_UndefinedValue_ReturnsDefault_And_InvokesCallback()
    {
        Type? reportedType = null;
        int? reportedValue = null;
        SafeEnumJsonConverterFactory.UnknownEnumNumberToken = (t, v) => { reportedType = t; reportedValue = v; };

        var result = JsonSerializer.Deserialize<NonNullableModel>("{\"Color\":99}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(default(Color)));
        Assert.That(reportedType, Is.EqualTo(typeof(Color)));
        Assert.That(reportedValue, Is.EqualTo(99));
    }

    // --- Non-nullable null token ---

    [Test]
    public void Read_NullToken_NonNullableEnum_ReturnsDefault()
    {
        var result = JsonSerializer.Deserialize<NonNullableModel>("{\"Color\":null}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(default(Color)));
    }

    // --- Nullable enum deserialization ---

    [Test]
    public void Read_NullToken_NullableEnum_ReturnsNull()
    {
        var result = JsonSerializer.Deserialize<NullableModel>("{\"Color\":null}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.Null);
    }

    [Test]
    public void Read_StringToken_NullableEnum_ParsesValue()
    {
        var result = JsonSerializer.Deserialize<NullableModel>("{\"Color\":\"Blue\"}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(Color.Blue));
    }

    [Test]
    public void Read_StringToken_NullableEnum_UnknownValue_ReturnsDefault()
    {
        var result = JsonSerializer.Deserialize<NullableModel>("{\"Color\":\"Yellow\"}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(default(Color)));
    }

    [Test]
    public void Read_NumberToken_NullableEnum_DefinedValue_ParsesValue()
    {
        var result = JsonSerializer.Deserialize<NullableModel>("{\"Color\":1}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(Color.Green));
    }

    // --- Serialization (non-nullable) ---

    [Test]
    public void Write_NonNullableEnum_WritesString()
    {
        var json = JsonSerializer.Serialize(new NonNullableModel { Color = Color.Green }, _options);

        Assert.That(json, Does.Contain("\"Green\""));
    }

    // --- Serialization (nullable) ---

    [Test]
    public void Write_NullableEnum_WithValue_WritesString()
    {
        var json = JsonSerializer.Serialize(new NullableModel { Color = Color.Blue }, _options);

        Assert.That(json, Does.Contain("\"Blue\""));
    }

    [Test]
    public void Write_NullableEnum_Null_WritesNull()
    {
        var json = JsonSerializer.Serialize(new NullableModel { Color = null }, _options);

        Assert.That(json, Does.Contain("null"));
    }

    // --- Unsupported token types ---

    [Test]
    public void Read_BooleanToken_ReturnsDefault()
    {
        var result = JsonSerializer.Deserialize<NonNullableModel>("{\"Color\":true}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(default(Color)));
    }

    [Test]
    public void Read_ObjectToken_ReturnsDefault()
    {
        var result = JsonSerializer.Deserialize<NonNullableModel>("{\"Color\":{}}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(default(Color)));
    }

    [Test]
    public void Read_ArrayToken_ReturnsDefault()
    {
        var result = JsonSerializer.Deserialize<NonNullableModel>("{\"Color\":[]}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(default(Color)));
    }

    // --- Callbacks not set (no-op path) ---

    [Test]
    public void Read_UnknownString_NoCallback_DoesNotThrow()
    {
        SafeEnumJsonConverterFactory.UnknownEnumStringToken = null;

        var result = JsonSerializer.Deserialize<NonNullableModel>("{\"Color\":\"Yellow\"}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(default(Color)));
    }

    [Test]
    public void Read_UndefinedNumber_NoCallback_DoesNotThrow()
    {
        SafeEnumJsonConverterFactory.UnknownEnumNumberToken = null;

        var result = JsonSerializer.Deserialize<NonNullableModel>("{\"Color\":99}", _options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Color, Is.EqualTo(default(Color)));
    }

    // --- CreateConverter ---

    [Test]
    public void CreateConverter_NonNullableEnum_ReturnsConverter()
    {
        var factory = new SafeEnumJsonConverterFactory();
        var converter = factory.CreateConverter(typeof(Color), _options);

        Assert.That(converter, Is.Not.Null);
        Assert.That(converter, Is.InstanceOf<JsonConverter<Color>>());
    }

    [Test]
    public void CreateConverter_NullableEnum_ReturnsNullableConverter()
    {
        var factory = new SafeEnumJsonConverterFactory();
        var converter = factory.CreateConverter(typeof(Color?), _options);

        Assert.That(converter, Is.Not.Null);
        Assert.That(converter, Is.InstanceOf<JsonConverter<Color?>>());
    }
}
