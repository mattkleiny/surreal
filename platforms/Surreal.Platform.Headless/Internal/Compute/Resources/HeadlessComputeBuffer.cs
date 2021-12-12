using Surreal.Compute.Memory;
using Surreal.Memory;

namespace Surreal.Internal.Compute.Resources;

internal sealed class HeadlessComputeBuffer<T> : ComputeBuffer<T>
  where T : unmanaged
{
  public override Memory<T> Read(Range range)
  {
    return Memory<T>.Empty;
  }

  public override void Write(ReadOnlySpan<T> data)
  {
    // no-op
  }
}
