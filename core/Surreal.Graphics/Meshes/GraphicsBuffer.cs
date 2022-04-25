using Surreal.Memory;

namespace Surreal.Graphics.Meshes;

/// <summary>Different usage models for buffers.</summary>
public enum BufferUsage
{
  Static,
  Dynamic,
}

/// <summary>A buffer of data on the <see cref="IGraphicsServer"/>.</summary>
public abstract class GraphicsBuffer : GraphicsResource, IHasSizeEstimate
{
  public abstract Type Type { get; }

  public int  Length { get; protected set; }
  public Size Size   { get; protected set; }
}

/// <summary>A strongly-typed <see cref="GraphicsBuffer"/> of <see cref="T"/>.</summary>
public sealed class GraphicsBuffer<T> : GraphicsBuffer, IDisposableBuffer<T>
  where T : unmanaged
{
  private readonly IGraphicsServer server;

  public GraphicsBuffer(IGraphicsServer server, BufferUsage usage = BufferUsage.Static)
  {
    this.server = server;

    Usage  = usage;
    Handle = server.CreateBuffer();
  }

  public override Type Type => typeof(T);

  public GraphicsHandle Handle { get; }
  public BufferUsage    Usage  { get; }

  public Memory<T> Read(Optional<Range> range = default)
  {
    return server.ReadBufferData<T>(Handle, range.GetOrDefault(Range.All));
  }

  public void Write(ReadOnlySpan<T> buffer)
  {
    Length = buffer.Length;
    Size   = buffer.CalculateSize();

    server.WriteBufferData(Handle, buffer, Usage);
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
