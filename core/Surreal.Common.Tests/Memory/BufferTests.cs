namespace Surreal.Memory;

public class BufferTests
{
  [Test]
  public void it_should_allocate_a_standard_buffer()
  {
    var buffer = Buffers.Allocate<uint>(100);

    FillBuffer(buffer.Data.Span);
  }

  [Test]
  public void it_should_allocate_a_pinned_buffer()
  {
    var buffer = Buffers.AllocatePinned<uint>(100);

    FillBuffer(buffer.Data.Span);
  }

  [Test]
  public void it_should_allocate_and_free_a_native_buffer()
  {
    using var buffer = Buffers.AllocateNative<uint>(100);

    FillBuffer(buffer.Data.Span);
  }

  private static void FillBuffer(Span<uint> span)
  {
    for (var i = 0; i < span.Length; i++)
    {
      span[i] = (uint) i;
    }
  }
}
