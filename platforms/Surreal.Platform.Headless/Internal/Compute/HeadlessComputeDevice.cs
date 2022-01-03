using Surreal.Compute;
using Surreal.Compute.Execution;
using Surreal.Compute.Memory;
using Surreal.Internal.Compute.Resources;

namespace Surreal.Internal.Compute;

internal sealed class HeadlessComputeDevice : IComputeDevice
{
  public ComputeBuffer<T> CreateBuffer<T>() where T : unmanaged
  {
    return new HeadlessComputeBuffer<T>();
  }

  public ComputeProgram CreateProgram(ReadOnlySpan<byte> raw)
  {
    return new HeadlessComputeProgram();
  }
}
