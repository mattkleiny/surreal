using System;
using Surreal.Compute.Execution;
using Surreal.Compute.Memory;

namespace Surreal.Compute.SPI {
  public interface IComputeBackend {
    ComputeBuffer  CreateBuffer(int width, int height);
    ComputeProgram CreateProgram(ReadOnlySpan<byte> raw);
  }
}