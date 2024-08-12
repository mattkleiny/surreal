namespace Surreal.Collections;

public class RingBufferTests
{
  [Test]
  public void it_should_store_elements()
  {
    var buffer = new RingBuffer<int>(3);

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
    var buffer = new RingBuffer<int>(3);

    for (var i = 0; i < 1000; i++)
    {
      buffer.Add(i);
    }

    buffer.Count.Should().Be(3);
  }

  [Test]
  public void it_should_clear()
  {
    var buffer = new RingBuffer<int>(3);

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
    var buffer = new RingBuffer<int>(3);

    for (var i = 0; i < 1000; i++)
    {
      buffer.Add(i);
    }

    var results = buffer.ToArray();

    results.Should().BeEquivalentTo(new[] { 999, 998, 997 });
  }

  [Test]
  public void it_should_compute_sum_efficiently()
  {
    var buffer1 = new RingBuffer<int>(3) { 1, 2, 3 };
    var buffer2 = new RingBuffer<float>(3) { 1, 2, 3 };
    var buffer3 = new RingBuffer<double>(3) { 1, 2, 3 };
    var buffer4 = new RingBuffer<TimeSpan>(3) { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3) };

    buffer1.FastSum().Should().Be(6);
    buffer2.FastSum().Should().Be(6);
    buffer3.FastSum().Should().Be(6);
    buffer4.FastSum().Should().Be(TimeSpan.FromSeconds(6));
  }

  [Test]
  public void it_should_compute_average_efficiently()
  {
    var buffer1 = new RingBuffer<int>(3) { 1, 2, 3 };
    var buffer2 = new RingBuffer<float>(3) { 1, 2, 3 };
    var buffer3 = new RingBuffer<double>(3) { 1, 2, 3 };
    var buffer4 = new RingBuffer<TimeSpan>(3) { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3) };

    buffer1.FastAverage().Should().Be(2);
    buffer2.FastAverage().Should().Be(2);
    buffer3.FastAverage().Should().Be(2);
    buffer4.FastAverage().Should().Be(TimeSpan.FromSeconds(2));
  }

  [Test]
  public void it_should_compute_min_efficiently()
  {
    var buffer1 = new RingBuffer<int>(3) { 1, 2, 3 };
    var buffer2 = new RingBuffer<float>(3) { 1, 2, 3 };
    var buffer3 = new RingBuffer<double>(3) { 1, 2, 3 };
    var buffer4 = new RingBuffer<TimeSpan>(3) { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3) };

    buffer1.FastMin().Should().Be(1);
    buffer2.FastMin().Should().Be(1);
    buffer3.FastMin().Should().Be(1);
    buffer4.FastMin().Should().Be(TimeSpan.FromSeconds(1));
  }

  [Test]
  public void it_should_compute_max_efficiently()
  {
    var buffer1 = new RingBuffer<int>(3) { 1, 2, 3 };
    var buffer2 = new RingBuffer<float>(3) { 1, 2, 3 };
    var buffer3 = new RingBuffer<double>(3) { 1, 2, 3 };
    var buffer4 = new RingBuffer<TimeSpan>(3) { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3) };

    buffer1.FastMax().Should().Be(3);
    buffer2.FastMax().Should().Be(3);
    buffer3.FastMax().Should().Be(3);
    buffer4.FastMax().Should().Be(TimeSpan.FromSeconds(3));
  }
}