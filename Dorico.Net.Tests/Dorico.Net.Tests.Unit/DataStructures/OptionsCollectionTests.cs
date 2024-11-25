using DoricoNet.DataStructures;
using DoricoNet.Responses;
using System.Diagnostics.CodeAnalysis;

namespace Dorico.Net.Tests.Unit.DataStructures;

[ExcludeFromCodeCoverage]
[TestFixture]
public class OptionsCollectionTests
{
    private OptionInfo _option1;
    private OptionInfo _option2;
    private OptionInfo _option3;
    private OptionCollection _optionsList;

    [SetUp]
    public void Setup()
    {
        _option1 = new("Path1.A", "ValueType1", "CurrentValue1", ["EnumValue1", "EnumValue2"]);
        _option2 = new("Path1.B", "ValueType2", "CurrentValue2", ["EnumValue3", "EnumValue4"]);
        _option3 = new("Path2.A", "ValueType3", "CurrentValue3", ["EnumValue3", "EnumValue4"]);

        _optionsList =
        [
            _option1,
            _option2,
            _option3
        ];
    }

    [Test]
    public void OptionsList_ValidName_ReturnsCommandInfo()
    {
        var optionInfo = _optionsList["Path1.A"];

        Assert.Multiple(() =>
        {
            Assert.That(optionInfo.Path, Is.EqualTo("Path1.A"));
            Assert.That(optionInfo.ValueType, Is.EqualTo("ValueType1"));
            Assert.That(optionInfo.CurrentValue, Is.EqualTo("CurrentValue1"));
            Assert.That(optionInfo.EnumValues?.First(), Is.EqualTo("EnumValue1"));
        });
    }

    [Test]
    public void OptionsList_Indexer_KeyNotFoundException()
    {
        Assert.Throws<KeyNotFoundException>(() => { var result = _optionsList["Path2.B"]; });
    }

    [Test]
    public void OptionsList_ReturnsOrganizedTreeListNode()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_optionsList.OrganizedItems, Is.Not.Null);
            Assert.That(_optionsList.OrganizedItems!.ChildNodes, Has.Count.EqualTo(2));
            Assert.That(_optionsList.OrganizedItems!.ChildNodes[0].Path, Is.EqualTo("Path1"));
            Assert.That(_optionsList.OrganizedItems!.ChildNodes[0].Values, Has.Count.EqualTo(2));
            Assert.That(_optionsList.OrganizedItems!.ChildNodes[1].Path, Is.EqualTo("Path2"));
            Assert.That(_optionsList.OrganizedItems!.ChildNodes[1].Values, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void OptionsList_Contains()
    {
        var contains = _optionsList.Contains(_option2);
        Assert.That(contains, Is.True);
    }

    [Test]
    public void OptionsList_GetEnumerator_Generic_ReturnsCorrectEnumerator()
    {
        var enumerator = _optionsList.GetEnumerator();

        Assert.Multiple(() =>
        {
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(_option1));
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.MoveNext(), Is.False);
        });
    }

    [Test]
    public void OptionsList_GetEnumerator_NonGeneric_ReturnsCorrectEnumerator()
    {
        var enumerator = ((System.Collections.IEnumerable)_optionsList).GetEnumerator();

        Assert.Multiple(() =>
        {
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(_option1));
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.MoveNext(), Is.False);
        });
    }

    [Test]
    public void OptionsList_Remove_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() => _optionsList.Remove(_option1));
    }

    [Test]
    public void OptionsList_Clear_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() => _optionsList.Clear());
    }

    [Test]
    public void OptionsList_CopyTo_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() => _optionsList.CopyTo(new OptionInfo[1], 0));
    }
}
