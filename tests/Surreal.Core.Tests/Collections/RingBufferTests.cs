using System.Linq;
using Surreal.Collections;
using Xunit;

namespace Surreal.Core.Collections {
  public class RingBufferTests {
    private readonly RingBuffer<int> buffer = new RingBuffer<int>(capacity: 3);

    [Fact]
    public void it_should_store_elements() {
      Assert.Equal(0, buffer.Count);
      buffer.Add(1);
      Assert.Equal(1, buffer.Count);
      buffer.Add(2);
      Assert.Equal(2, buffer.Count);
      buffer.Add(3);
      Assert.Equal(3, buffer.Count);
      buffer.Add(4);
      Assert.Equal(3, buffer.Count);
    }

    [Fact]
    public void it_should_reuse_existing_element_spots() {
      for (var i = 0; i < 1000; i++) buffer.Add(i);
      Assert.Equal(3, buffer.Count);
    }

    [Fact]
    public void it_should_clear() {
      for (var i = 0; i < 1000; i++) buffer.Add(i);
      Assert.Equal(3, buffer.Count);
      buffer.Clear();
      Assert.Equal(0, buffer.Count);
    }

    [Fact]
    public void it_should_enumerate_in_reverse_insertion_order() {
      for (var i = 0; i < 1000; i++) buffer.Add(i);

      var results = buffer.ToArray();

      Assert.Equal(results, new[] {999, 998, 997});
    }
  }
}