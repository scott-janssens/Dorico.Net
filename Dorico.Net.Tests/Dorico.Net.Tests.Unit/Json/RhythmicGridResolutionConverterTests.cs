﻿using DoricoNet.Enums;
using DoricoNet.Json;
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
    public void RhythmicGridResolutionConverter_ReadNull()
    {
        var value = (Test?)JsonSerializer.Deserialize(
            "{\"resolution\": null}",
            typeof(Test),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.That(value, Is.Not.Null);
        Assert.That(value.Resolution, Is.Null);
    }

    [Test]
    public void RhythmicGridResolutionConverter_ReadEmpty()
    {
        var value = (Test?)JsonSerializer.Deserialize(
            "{\"resolution\": \"\"}",
            typeof(Test),
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        Assert.That(value, Is.Not.Null);
        Assert.That(value.Resolution, Is.EqualTo(RhythmicGridResolution.Undefined));
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

    [Test]
    public void RhythmicGridResolutionConverter_WriteNull()
    {
        var value = JsonSerializer.Serialize(
            new Test { Resolution = null },
            typeof(Test),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.That(value, Is.EqualTo("{\"resolution\":null}"));
    }

    [Test]
    public void RhythmicGridResolutionConverter_WriteEmpty()
    {
        var value = JsonSerializer.Serialize(
            new Test { Resolution = RhythmicGridResolution.Undefined },
            typeof(Test),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.That(value, Is.EqualTo("{\"resolution\":\"Undefined\"}"));
    }

    private class Test
    {
        public RhythmicGridResolution? Resolution { get; set; }
    }
}