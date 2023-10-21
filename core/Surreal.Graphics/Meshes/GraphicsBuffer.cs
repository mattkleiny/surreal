using Surreal.Memory;

namespace Surreal.Graphics.Meshes;

/// <summary>
/// Different types of <see cref="GraphicsBuffer" />s.
/// </summary>
public enum BufferType
{
  Vertex,
  Index
}

/// <summary>
/// Different usage models for buffers.
/// </summary>
public enum BufferUsage
{
  Static,
  Dynamic
}

/// <summary>
/// A buffer of data on the <see cref="GraphicsContext" />.
/// </summary>
public abstract class GraphicsBuffer : GraphicsResource, IHasSizeEstimate
{
  public abstract Type ElementType { get; }

  public int Length { get; protected set; }
  public Size Size { get; protected set; }
}

/// <summary>
/// A strongly-typed <see cref="GraphicsBuffer" /> of <see cref="T" />.
/// </summary>
public sealed class GraphicsBuffer<T>(GraphicsContext context, BufferType type, BufferUsage usage = BufferUsage.Static) : GraphicsBuffer
  where T : unmanaged
{
  public override Type ElementType => typeof(T);

  public GraphicsHandle Handle { get; } = context.Backend.CreateBuffer(type);
  public BufferType Type { get; } = type;
  public BufferUsage Usage { get; } = usage;

  public Memory<T> Read(Optional<Range> range = default)
  {
    var (offset, length) = range
      .GetOrDefault(Range.All)
      .GetOffsetAndLength(Length);

    return context.Backend.ReadBufferData<T>(Handle, Type, offset, length);
  }

  public void Write(ReadOnlySpan<T> buffer)
  {
    Length = buffer.Length;
    Size = buffer.CalculateSize();

    context.Backend.WriteBufferData(Handle, Type, buffer, Usage);
  }

  public void Write(nint offset, ReadOnlySpan<T> buffer)
  {
    context.Backend.WriteBufferSubData(Handle, Type, offset, buffer);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      context.Backend.DeleteBuffer(Handle);
    }

    base.Dispose(managed);
  }
}
