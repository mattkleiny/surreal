using Surreal.Compute;
using Surreal.Compute.Execution;
using Surreal.Compute.Memory;
using Surreal.Internal.Compute.Resources;

namespace Surreal.Internal.Compute;

internal sealed class OpenTKComputeDevice : IComputeDevice
{
  public ComputeBuffer<T> CreateBuffer<T>()
    where T : unmanaged
  {
    return new OpenTKComputeBuffer<T>();
  }

  public ComputeProgram CreateProgram(ReadOnlySpan<byte> raw)
  {
    return new OpenTKComputeProgram(raw);
  }
}
