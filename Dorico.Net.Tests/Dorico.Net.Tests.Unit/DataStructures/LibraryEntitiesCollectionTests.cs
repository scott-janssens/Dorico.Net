using DoricoNet.DataStructures;
using DoricoNet.Responses;
using System.Diagnostics.CodeAnalysis;

namespace Dorico.Net.Tests.DataStructures;

[ExcludeFromCodeCoverage]
[TestFixture]
public class LibraryEntitiesCollectionTests
{
    private LibraryEntityCollection _collection;
    private LibraryEntity _entity;

    [SetUp]
    public void SetUp()
    {
        _collection = new LibraryEntityCollection();
        _entity = new LibraryEntity("1", "One", null);
    }

    [Test]
    public void Add_AddsEntityToCollection()
    {
        _collection.Add(_entity);

        Assert.That(_collection, Has.Count.EqualTo(1));
        Assert.That(_collection, Does.Contain(_entity));
    }

    [Test]
    public void GetByEntityId_ReturnsCorrectEntity()
    {
        _collection.Add(_entity);

        var result = _collection.GetByEntityId(_entity.EntityId);

        Assert.That(result, Is.EqualTo(_entity));
    }

    [Test]
    public void GetByName_ReturnsCorrectEntities()
    {
        _collection.Add(_entity);

        var result = _collection.GetByName(_entity.Name);

        Assert.That(result, Contains.Item(_entity));
    }

    [Test]
    public void LibraryEntities_Contains()
    {
        _collection.Add(_entity);
        var contains = _collection.Contains(_entity);
        Assert.That(contains, Is.True);
    }

    [Test]
    public void Remove_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() => _collection.Remove(_entity));
    }

    [Test]
    public void Clear_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() => _collection.Clear());
    }

    [Test]
    public void CopyTo_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() => _collection.CopyTo(new LibraryEntity[1], 0));
    }

    [Test]
    public void GetEnumerator_Generic_ReturnsCorrectEnumerator()
    {
        _collection.Add(_entity);

        var enumerator = _collection.GetEnumerator();

        Assert.Multiple(() =>
        {
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(_entity));
            Assert.That(enumerator.MoveNext(), Is.False);
        });
    }

    [Test]
    public void GetEnumerator_NonGeneric_ReturnsCorrectEnumerator()
    {
        _collection.Add(_entity);

        var enumerator = ((System.Collections.IEnumerable)_collection).GetEnumerator();

        Assert.Multiple(() =>
        {
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.Current, Is.EqualTo(_entity));
            Assert.That(enumerator.MoveNext(), Is.False);
        });
    }
}
