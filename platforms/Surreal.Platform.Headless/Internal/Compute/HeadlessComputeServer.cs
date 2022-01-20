using Surreal.Compute;
using Surreal.Compute.Execution;
using Surreal.Compute.Memory;
using Surreal.Internal.Compute.Resources;

namespace Surreal.Internal.Compute;

internal sealed class HeadlessComputeServer : IComputeServer
{
  public ComputeBuffer<T> CreateBuffer<T>() where T : unmanaged
  {
    return new HeadlessComputeBuffer<T>();
  }

  public ComputeProgram CreateProgram()
  {
    return new HeadlessComputeProgram();
  }
}
