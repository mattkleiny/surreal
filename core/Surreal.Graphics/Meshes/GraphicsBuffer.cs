using System.Buffers;
using Surreal.Memory;

namespace Surreal.Graphics.Meshes;

/// <summary>Different types of <see cref="GraphicsBuffer"/>s.</summary>
public enum BufferType
{
  Vertex,
  Index
}

/// <summary>Different usage models for buffers.</summary>
public enum BufferUsage
{
  Static,
  Dynamic
}

/// <summary>A buffer of data on the <see cref="IGraphicsServer"/>.</summary>
public abstract class GraphicsBuffer : GraphicsResource, IHasSizeEstimate
{
  public abstract Type ElementType { get; }

  public int  Length { get; protected set; }
  public Size Size   { get; protected set; }
}

/// <summary>A strongly-typed <see cref="GraphicsBuffer"/> of <see cref="T"/>.</summary>
public sealed class GraphicsBuffer<T> : GraphicsBuffer
  where T : unmanaged
{
  private readonly IGraphicsServer server;

  public GraphicsBuffer(IGraphicsServer server, BufferType type, BufferUsage usage = BufferUsage.Static)
  {
    this.server = server;

    Type   = type;
    Usage  = usage;
    Handle = server.CreateBuffer(type);
  }

  public override Type ElementType => typeof(T);

  public GraphicsHandle Handle { get; }
  public BufferType     Type   { get; }
  public BufferUsage    Usage  { get; }

  public Span<T> this[Range range] => Read(range).Span;

  public BufferDataLease Rent()
  {
    return new BufferDataLease(this);
  }

  public Memory<T> Read(Optional<Range> range = default)
  {
    var (offset, length) = range
      .GetOrDefault(Range.All)
      .GetOffsetAndLength(Length);

    return server.ReadBufferData<T>(Handle, Type, offset, length);
  }

  public void Write(ReadOnlySpan<T> buffer)
  {
    Length = buffer.Length;
    Size   = buffer.CalculateSize();

    server.WriteBufferData(Handle, Type, buffer, Usage);
  }

  public void Write(nint offset, ReadOnlySpan<T> buffer)
  {
    server.WriteBufferSubData(Handle, Type, offset, buffer);
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
