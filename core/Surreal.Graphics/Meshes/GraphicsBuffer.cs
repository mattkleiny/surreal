using Surreal.Memory;

namespace Surreal.Graphics.Meshes;

/// <summary>A buffer of data on the <see cref="IGraphicsServer"/>.</summary>
public abstract class GraphicsBuffer : GraphicsResource, IHasSizeEstimate
{
  public int Length { get; protected set; }
  public Size Size { get; protected set; }
}

/// <summary>A strongly-typed <see cref="GraphicsBuffer"/> of <see cref="T"/>.</summary>
public sealed class GraphicsBuffer<T> : GraphicsBuffer, IDisposableBuffer<T>
  where T : unmanaged
{
  private readonly GraphicsHandle handle;
  private readonly IGraphicsServer server;

  public GraphicsBuffer(IGraphicsServer server)
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
