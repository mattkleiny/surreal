using System;
using Surreal.Graphics.Meshes;

namespace Surreal.Platform.Internal.Graphics.Resources {
  internal sealed class HeadlessGraphicsBuffer : GraphicsBuffer {
    public override Span<T> Read<T>(Range range) {
      return Span<T>.Empty;
    }

    public override void Write<T>(Span<T> data) {
      // no-op
    }
  }
}