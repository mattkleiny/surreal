namespace Surreal.Collections;

public class RingBufferTests
{
  [Test]
  public void it_should_store_elements()
  {
    var buffer = new RingBuffer<int>(capacity: 3);

    buffer.Count.Should().Be(0);
    buffer.Add(1);
    buffer.Count.Should().Be(1);
    buffer.Add(2);
    buffer.Count.Should().Be(2);
    buffer.Add(3);
    buffer.Count.Should().Be(3);
    buffer.Add(4);
    buffer.Count.Should().Be(3);
  }

  [Test]
  public void it_should_reuse_existing_element_spots()
  {
    var buffer = new RingBuffer<int>(capacity: 3);

    for (var i = 0; i < 1000; i++)
    {
      buffer.Add(i);
    }

    buffer.Count.Should().Be(3);
  }

  [Test]
  public void it_should_clear()
  {
    var buffer = new RingBuffer<int>(capacity: 3);

    for (var i = 0; i < 1000; i++)
    {
      buffer.Add(i);
    }

    buffer.Count.Should().Be(3);
    buffer.Clear();
    buffer.Count.Should().Be(0);
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

    results.Should().BeEquivalentTo(new[] { 999, 998, 997 });
  }
}
