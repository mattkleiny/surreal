using System;
using Surreal.Graphics.Meshes;

namespace Surreal.Platform.Internal.Graphics.Resources {
  internal sealed class HeadlessGraphicsBuffer<T> : GraphicsBuffer<T>
      where T : unmanaged {
    public override Memory<T> Read(Range range) {
      return Memory<T>.Empty;
    }

    public override void Write(Span<T> data) {
      // no-op
    }
  }
}