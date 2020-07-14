using System;
using Surreal.Compute.Execution;
using Surreal.Compute.Memory;

namespace Surreal.Compute {
  public interface IComputeDevice {
    ComputeBuffer  CreateBuffer();
    ComputeProgram CreateProgram(ReadOnlySpan<byte> raw);
  }
}