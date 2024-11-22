using DoricoNet.Enums;
using DoricoNet.Json;
using DoricoNet.Responses;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Dorico.Net.Tests.Unit.Json;

[ExcludeFromCodeCoverage]
[TestFixture]
internal class RhythmicGridResolutionConverterTests
{
    private RhythmicGridResolutionConverter _converter;

    [SetUp]
    public void Setup()
    {
        _converter = new();
    }

    [Test]
    public void RhythmicGridResolutionConverter_Read()
    {
        var value = (Test?)JsonSerializer.Deserialize(
            "{\"resolution\": \"kCrotchet\"}",
            typeof(Test),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.That(value, Is.Not.Null);
        Assert.That(value.Resolution, Is.EqualTo(RhythmicGridResolution.kCrotchet));
    }

    [Test]
    public void RhythmicGridResolutionConverter_Write()
    {
        var value = JsonSerializer.Serialize(
            new Test { Resolution = RhythmicGridResolution.Quarter },
            typeof(Test),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.That(value, Is.EqualTo("{\"resolution\":\"kCrotchet\"}"));
    }

    private class Test
    {
        public RhythmicGridResolution Resolution { get; set; }
    }
}