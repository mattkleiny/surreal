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
public abstract class GraphicsBuffer : Disposable
{
  /// <summary>
  /// The type of element stored in the <see cref="GraphicsBuffer" />.
  /// </summary>
  public abstract Type DataType { get; }

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
public sealed class GraphicsBuffer<T>(IGraphicsDevice device, BufferType type, BufferUsage usage = BufferUsage.Static) : GraphicsBuffer
  where T : unmanaged
{
  /// <inheritdoc/>
  public override Type DataType => typeof(T);

  /// <summary>
  /// The <see cref="GraphicsHandle"/> for the underlying buffer.
  /// </summary>
  public GraphicsHandle Handle { get; } = device.CreateBuffer(type, usage);

  /// <summary>
  /// The type of the <see cref="GraphicsBuffer" />.
  /// </summary>
  public BufferType Type { get; } = type;

  /// <summary>
  /// The usage intention of the <see cref="GraphicsBuffer" />.
  /// </summary>
  public BufferUsage Usage { get; } = usage;

  /// <summary>
  /// Reads the buffer into a <see cref="Memory{T}" />.
  /// </summary>
  public GraphicsTask<Memory<T>> ReadAsync()
  {
    return device.ReadBufferDataAsync<T>(Handle, Type);
  }

  /// <summary>
  /// Writes the given <see cref="ReadOnlySpan{T}" /> to the buffer.
  /// </summary>
  public GraphicsTask WriteAsync(ReadOnlySpan<T> span)
  {
    Length = (uint)span.Length;
    Size = span.CalculateSize();

    return device.WriteBufferDataAsync(Handle, Type, span, Usage);
  }

  /// <summary>
  /// Writes the given <see cref="ReadOnlySpan{T}" /> to the buffer at the given offset.
  /// </summary>
  public GraphicsTask WriteAsync(uint offset, ReadOnlySpan<T> span)
  {
    return device.WriteBufferDataAsync(Handle, Type, offset, span);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      device.DeleteBuffer(Handle);
    }

    base.Dispose(managed);
  }
}
