using System;
using Surreal.Compute.Execution;
using Surreal.Compute.Memory;

namespace Surreal.Compute.SPI {
  public interface IComputeFactory {
    ComputeBuffer  CreateBuffer(int width, int height);
    ComputeProgram CreateProgram(ReadOnlySpan<byte> raw);
  }
}