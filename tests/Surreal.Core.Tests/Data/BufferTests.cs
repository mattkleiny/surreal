using System.Numerics;
using Surreal.Mathematics.Tensors;
using Surreal.Memory;
using Xunit;

namespace Surreal.Core.Data {
  public class BufferTests {
    [Fact]
    public void it_should_allocate_memory_on_the_heap_correctly() {
      TestReadWrite(Buffers.Allocate<Vector2>(16 * 16));
    }

    [Fact]
    public void it_should_allocate_memory_off_the_heap_correctly() {
      using (var memory = Buffers.AllocateOffHeap<Vector2>(16 * 16)) {
        TestReadWrite(memory);
      }
    }

    [Fact]
    public void it_should_allocate_memory_from_a_pool_correctly() {
      var pool = Buffers.AllocatePool<Vector2>(maxBufferSize: 16 * 16, bucketSize: 1);

      for (var i = 0; i < 1000; i++) {
        using (var memory = pool.Rent(16 * 16)) {
          TestReadWrite(memory);
        }
      }
    }

    private static void TestReadWrite(IBuffer<Vector2> buffer) {
      var tensor = new Tensor2D<Vector2>(buffer, 16, 16);

      var identity = new Vector2(1, 1);

      for (var y = 0; y < tensor.Width; y++)
      for (var x = 0; x < tensor.Height; x++) {
        tensor[x, y] = identity;
      }

      Assert.Equal(identity, tensor[0, 0]);
      Assert.Equal(identity, tensor[15, 15]);
    }
  }
}