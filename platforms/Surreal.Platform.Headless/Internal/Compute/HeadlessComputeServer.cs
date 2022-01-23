using Surreal.Compute;

namespace Surreal.Internal.Compute;

internal sealed class HeadlessComputeServer : IComputeServer
{
  private int nextBufferId = 0;

  public ComputeHandle CreateBuffer()
  {
    return new ComputeHandle(Interlocked.Increment(ref nextBufferId));
  }

  public void DeleteBuffer(ComputeHandle handle)
  {
    // no-op
  }

  public Memory<T> ReadBufferData<T>(ComputeHandle handle, Range range) where T : unmanaged
  {
    return Memory<T>.Empty;
  }

  public void WriteBufferData<T>(ComputeHandle handle, ReadOnlySpan<T> data) where T : unmanaged
  {
    // no-op
  }
}
