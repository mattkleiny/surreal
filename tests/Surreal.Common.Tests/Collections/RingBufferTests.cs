using NUnit.Framework;

namespace Surreal.Collections;

public class RingBufferTests
{
  [Test]
  public void it_should_store_elements()
  {
    var buffer = new RingBuffer<int>(capacity: 3);

    Assert.AreEqual(0, buffer.Count);
    buffer.Add(1);
    Assert.AreEqual(1, buffer.Count);
    buffer.Add(2);
    Assert.AreEqual(2, buffer.Count);
    buffer.Add(3);
    Assert.AreEqual(3, buffer.Count);
    buffer.Add(4);
    Assert.AreEqual(3, buffer.Count);
  }

  [Test]
  public void it_should_reuse_existing_element_spots()
  {
    var buffer = new RingBuffer<int>(capacity: 3);

    for (var i = 0; i < 1000; i++)
    {
      buffer.Add(i);
    }

    Assert.AreEqual(3, buffer.Count);
  }

  [Test]
  public void it_should_clear()
  {
    var buffer = new RingBuffer<int>(capacity: 3);

    for (var i = 0; i < 1000; i++)
    {
      buffer.Add(i);
    }

    Assert.AreEqual(3, buffer.Count);
    buffer.Clear();
    Assert.AreEqual(0, buffer.Count);
  }

  [Test]
  public void it_should_enumerate_in_reverse_insertion_order()
  {
    var buffer = new RingBuffer<int>(capacity: 3);

    for (var i = 0; i < 1000; i++)
    {
      buffer.Add(i);
    }

    var results = buffer.ToArray();

    Assert.AreEqual(results, new[] { 999, 998, 997 });
  }
}
