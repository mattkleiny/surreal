namespace Surreal.Collections;

public class CollectionTests
{
  [Test]
  public void it_should_access_via_index()
  {
    var collection = new Collection<int> { 1, 2, 3 };

    collection[0].Should().Be(1);
    collection[1].Should().Be(2);
    collection[^1].Should().Be(3);
  }

  [Test]
  public void it_should_access_via_range()
  {
    var collection = new Collection<int> { 1, 2, 3, 4, 5 };

    collection[..2].Should().BeEquivalentTo(new[] { 1, 2 });
    collection[1..^1].Should().BeEquivalentTo(new[] { 2, 3, 4 });
  }

  [Test]
  public void it_should_notify_of_adding()
  {
    var addedCount = 0;

    var collection = new Collection<int>();

    collection.ItemAdded += (_, _) => addedCount += 1;

    collection.Add(1);
    collection.Add(2);

    addedCount.Should().Be(2);
  }

  [Test]
  public void it_should_notify_of_removing()
  {
    var removedCount = 0;

    var collection = new Collection<int>();

    collection.ItemRemoved += (_, _) => removedCount += 1;

    collection.Add(1);
    collection.Add(2);
    collection.Remove(1);
    collection.Remove(2);

    removedCount.Should().Be(2);
  }

  [Test]
  public void it_should_notify_of_clearing()
  {
    var removedCount = 0;

    var collection = new Collection<int>();

    collection.ItemRemoved += (_, _) => removedCount += 1;

    collection.Add(1);
    collection.Add(2);
    collection.Add(3);

    collection.Clear();

    removedCount.Should().Be(3);
  }

  [Test]
  public void it_should_notify_of_inserting()
  {
    var addedCount = 0;

    var collection = new Collection<int>();

    collection.ItemAdded += (_, _) => addedCount += 1;

    collection.Insert(0, 1);
    collection.Insert(0, 2);
    collection.Insert(0, 3);

    addedCount.Should().Be(3);
  }

  [Test]
  public void it_should_notify_of_removing_at()
  {
    var removedCount = 0;

    var collection = new Collection<int>();

    collection.ItemRemoved += (_, _) => removedCount += 1;

    collection.Add(1);
    collection.Add(2);
    collection.Add(3);

    collection.RemoveAt(0);
    collection.RemoveAt(0);
    collection.RemoveAt(0);

    removedCount.Should().Be(3);
  }
}
