using Surreal.Compute.Execution;
using Surreal.Compute.Memory;

namespace Surreal.Compute;

/// <summary>An opaque handle to a resource in the underling <see cref="IComputeServer"/> implementation.</summary>
public readonly record struct ComputeHandle(uint Id)
{
  public ComputeHandle(int id)
    : this((uint) id)
  {
  }

  public static implicit operator uint(ComputeHandle handle) => handle.Id;
  public static implicit operator int(ComputeHandle handle)  => (int) handle.Id;
}

/// <summary>Represents the compute subsystem.</summary>
public interface IComputeServer
{
  ComputeBuffer<T> CreateBuffer<T>()
    where T : unmanaged;

  ComputeProgram CreateProgram();
}
