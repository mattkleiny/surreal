using System;
using Surreal.Compute.Memory;

namespace Surreal.Platform.Internal.Compute.Resources
{
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
}
