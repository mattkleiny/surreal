using System;
using Surreal.Compute;
using Surreal.Compute.Execution;
using Surreal.Compute.Memory;
using Surreal.Platform.Internal.Compute.Resources;

namespace Surreal.Platform.Internal.Compute {
  internal sealed class OpenTKComputeDevice : IComputeDevice, IDisposable {
    public ComputeBuffer<T> CreateBuffer<T>() where T : unmanaged {
      return new OpenTKComputeBuffer<T>();
    }

    public ComputeProgram CreateProgram(ReadOnlySpan<byte> raw) {
      return new OpenTKComputeProgram(raw);
    }

    public void Dispose() {
    }
  }
}