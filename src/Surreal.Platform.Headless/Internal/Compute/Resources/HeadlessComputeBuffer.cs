using System;
using System.Runtime.CompilerServices;
using Surreal.Compute.Memory;
using Surreal.Graphics;

namespace Surreal.Platform.Internal.Compute.Resources {
  internal sealed class HeadlessComputeBuffer : ComputeBuffer {
    public HeadlessComputeBuffer()
        : base(Unsafe.SizeOf<Color>()) {
    }

    public override void Put<T>(Span<T> data) {
    }
  }
}