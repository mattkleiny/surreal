using System;
using System.Buffers;
using Surreal.Compute.Memory;

namespace Surreal.Platform.Internal.Compute.Resources {
  internal sealed class HeadlessComputeBuffer : ComputeBuffer {
    public override Memory<T> Read<T>(Range range) {
      return Memory<T>.Empty;
    }

    public override void Write<T>(Span<T> data) {
      // no-op
    }

    public override MemoryManager<T> Pin<T>() {
      throw new NotImplementedException();
    }
  }
}