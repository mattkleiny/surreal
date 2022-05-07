using System.Buffers;
using Surreal.Memory;

namespace Surreal.Graphics.Meshes;

/// <summary>Different usage models for buffers.</summary>
public enum BufferUsage
{
  Static,
  Dynamic
}

/// <summary>A buffer of data on the <see cref="IGraphicsServer"/>.</summary>
public abstract class GraphicsBuffer : GraphicsResource, IHasSizeEstimate
{
  public abstract Type Type { get; }

  public int  Length { get; protected set; }
  public Size Size   { get; protected set; }
}

/// <summary>A strongly-typed <see cref="GraphicsBuffer"/> of <see cref="T"/>.</summary>
public sealed class GraphicsBuffer<T> : GraphicsBuffer
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

  public Span<T> this[Range range] => Read(range).Span;

  public BufferDataLease Rent()
  {
    return new BufferDataLease(this);
  }

  public Memory<T> Read(Optional<Range> range = default)
  {
    var (offset, length) = range.GetOrDefault(Range.All).GetOffsetAndLength(Length);

    return server.ReadBufferData<T>(Handle, offset, length);
  }

  public void Write(ReadOnlySpan<T> buffer)
  {
    Length = buffer.Length;
    Size   = buffer.CalculateSize();

    server.WriteBufferData(Handle, buffer, Usage);
  }

  public void Write(nint offset, ReadOnlySpan<T> buffer)
  {
    server.WriteSubBufferData(Handle, offset, buffer);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      server.DeleteBuffer(Handle);
    }

    base.Dispose(managed);
  }

  /// <summary>Allows borrowing the <see cref="GraphicsBuffer"/> data.</summary>
  public readonly struct BufferDataLease : IMemoryOwner<T>
  {
    private readonly GraphicsBuffer<T> buffer;

    public BufferDataLease(GraphicsBuffer<T> buffer)
    {
      this.buffer = buffer;
      Memory      = buffer.Read();
    }

    public Memory<T> Memory { get; }
    public Span<T>   Span   => Memory.Span;

    public void Dispose()
    {
      buffer.Write(Memory.Span);
    }
  }
}
