using Surreal.Compute.Execution;
using Surreal.Compute.Memory;

namespace Surreal.Compute;

/// <summary>Represents the compute subsystem.</summary>
public interface IComputeServer
{
  ComputeBuffer<T> CreateBuffer<T>()
    where T : unmanaged;

  ComputeProgram CreateProgram();
}
