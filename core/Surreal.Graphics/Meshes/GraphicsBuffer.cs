using Surreal.Memory;

namespace Surreal.Graphics.Meshes;

/// <summary>A buffer of data on the <see cref="IGraphicsServer"/>.</summary>
public abstract class GraphicsBuffer : GraphicsResource, IHasSizeEstimate
{
  public int  Length { get; protected set; }
  public Size Size   { get; protected set; }
}

/// <summary>A strongly-typed <see cref="GraphicsBuffer"/> of <see cref="T"/>.</summary>
public sealed class GraphicsBuffer<T> : GraphicsBuffer, IDisposableBuffer<T>
  where T : unmanaged
{
  private readonly IGraphicsServer server;

  public GraphicsBuffer(IGraphicsServer server)
  {
    this.server = server;

    Handle = server.CreateBuffer();
  }

  public GraphicsHandle Handle { get; }

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
