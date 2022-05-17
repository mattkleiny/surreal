namespace Surreal.Compute;

/// <summary>An opaque handle to a resource in the underling <see cref="IComputeServer"/> implementation.</summary>
public readonly record struct ComputeHandle(nint Id)
{
  public static ComputeHandle None => default;

  public static implicit operator nint(ComputeHandle handle) => handle.Id;
  public static implicit operator int(ComputeHandle handle) => (int) handle.Id;
  public static implicit operator uint(ComputeHandle handle) => (uint) handle.Id;
}

/// <summary>Represents the compute subsystem.</summary>
public interface IComputeServer
{
  // buffers
  ComputeHandle CreateBuffer();
  Memory<T> ReadBufferData<T>(ComputeHandle handle, nint offset, int length) where T : unmanaged;
  void WriteBufferData<T>(ComputeHandle handle, ReadOnlySpan<T> data) where T : unmanaged;
  void DeleteBuffer(ComputeHandle handle);
}
