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
/// A buffer of data on the <see cref="IGraphicsBackend" />.
/// </summary>
public abstract class GraphicsBuffer : GraphicsAsset, IHasSizeEstimate
{
  /// <summary>
  /// The type of element stored in the <see cref="GraphicsBuffer" />.
  /// </summary>
  public abstract Type ElementType { get; }

  /// <summary>
  /// The length of the <see cref="GraphicsBuffer" />.
  /// </summary>
  public uint Length { get; protected set; }

  /// <summary>
  /// The byte size of the <see cref="GraphicsBuffer" />.
  /// </summary>
  public Size Size { get; protected set; }
}

/// <summary>
/// A strongly-typed <see cref="GraphicsBuffer" /> of <see cref="T" />.
/// </summary>
public sealed class GraphicsBuffer<T>(IGraphicsBackend backend, BufferType type, BufferUsage usage = BufferUsage.Static) : GraphicsBuffer
  where T : unmanaged
{
  public override Type ElementType => typeof(T);

  /// <summary>
  /// The <see cref="GraphicsHandle"/> for the underlying buffer.
  /// </summary>
  public GraphicsHandle Handle { get; } = backend.CreateBuffer(type);

  /// <summary>
  /// The type of the <see cref="GraphicsBuffer" />.
  /// </summary>
  public BufferType Type { get; } = type;

  /// <summary>
  /// The usage intention of the <see cref="GraphicsBuffer" />.
  /// </summary>
  public BufferUsage Usage { get; } = usage;

  /// <summary>
  /// Reads the entire buffer into a <see cref="Memory{T}" />.
  /// </summary>
  public Memory<T> Read(Optional<Range> range = default)
  {
    var (offset, length) = range
      .GetOrDefault(Range.All)
      .GetOffsetAndLength((int)Length);

    return backend.ReadBufferData<T>(Handle, Type, offset, length);
  }

  /// <summary>
  /// Writes the given <see cref="ReadOnlySpan{T}" /> to the buffer.
  /// </summary>
  public void Write(ReadOnlySpan<T> buffer)
  {
    Length = (uint)buffer.Length;
    Size = buffer.CalculateSize();

    backend.WriteBufferData(Handle, Type, buffer, Usage);
  }

  /// <summary>
  /// Writes the given <see cref="ReadOnlySpan{T}" /> to the buffer at the given offset.
  /// </summary>
  public void Write(nint offset, ReadOnlySpan<T> buffer)
  {
    backend.WriteBufferSubData(Handle, Type, offset, buffer);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      backend.DeleteBuffer(Handle);
    }

    base.Dispose(managed);
  }
}
