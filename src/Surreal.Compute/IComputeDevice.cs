using System;
using Surreal.Compute.Execution;
using Surreal.Compute.Memory;

namespace Surreal.Compute
{
  public interface IComputeDevice
  {
    ComputeBuffer<T> CreateBuffer<T>() where T : unmanaged;
    ComputeProgram   CreateProgram(ReadOnlySpan<byte> raw);
  }
}