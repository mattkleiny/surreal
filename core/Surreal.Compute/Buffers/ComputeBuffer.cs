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
  private readonly IComputeServer server;

  public ComputeBuffer(IComputeServer server)
  {
    this.server = server;

    Handle = server.CreateBuffer();
  }

  public ComputeHandle Handle { get; }

  public Memory<T> Read(Optional<Range> range = default)
  {
    return server.ReadBufferData<T>(Handle, range.GetOrDefault(Range.All));
  }

  public void Write(ReadOnlySpan<T> buffer)
  {
    Length = buffer.Length;
    Size   = buffer.CalculateSize();

    server.WriteBufferData(Handle, buffer);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      server.DeleteBuffer(Handle);
    }

    base.Dispose(managed);
  }

  Memory<T> IBuffer<T>.Data => Read();
}
