using Surreal.Memory;

namespace Surreal.Compute.Buffers;

/// <summary>A buffer of data on the <see cref="IComputeServer"/>.</summary>
public abstract class ComputeBuffer : ComputeResource, IHasSizeEstimate
{
  public int  Length { get; protected set; }
  public Size Size   { get; protected set; }
}

/// <summary>A strongly-typed <see cref="ComputeBuffer"/> of <see cref="T"/>.</summary>
public sealed class ComputeBuffer<T> : ComputeBuffer, IDisposableBuffer<T>
  where T : unmanaged
{
  private readonly ComputeHandle handle;
  private readonly IComputeServer server;

  public ComputeBuffer(IComputeServer server)
  {
    this.server = server;

    handle = server.CreateBuffer();
  }

  public Memory<T> Read(Optional<Range> range = default)
  {
    return server.ReadBufferData<T>(handle, range.GetOrDefault(Range.All));
  }

  public void Write(ReadOnlySpan<T> buffer)
  {
    server.WriteBufferData(handle, buffer);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      server.DeleteBuffer(handle);
    }

    base.Dispose(managed);
  }

  Memory<T> IBuffer<T>.Data => Read();
}
