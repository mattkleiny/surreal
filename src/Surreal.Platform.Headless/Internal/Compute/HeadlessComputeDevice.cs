using System;
using Surreal.Compute;
using Surreal.Compute.Execution;
using Surreal.Compute.Memory;
using Surreal.Platform.Internal.Compute.Resources;

namespace Surreal.Platform.Internal.Compute {
  internal sealed class HeadlessComputeDevice : IComputeDevice {
    public ComputeBuffer CreateBuffer() {
      return new HeadlessComputeBuffer();
    }

    public ComputeProgram CreateProgram(ReadOnlySpan<byte> raw) {
      return new HeadlessComputeProgram();
    }
  }
}