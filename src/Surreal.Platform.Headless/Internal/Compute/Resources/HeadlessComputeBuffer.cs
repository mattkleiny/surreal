using System;
using Surreal.Compute.Memory;

namespace Surreal.Platform.Internal.Compute.Resources {
  internal sealed class HeadlessComputeBuffer : ComputeBuffer {
    public override Span<T> Read<T>(Range range) {
      return Span<T>.Empty;
    }

    public override void Write<T>(Span<T> data) {
    }
  }
}