using System.Runtime.CompilerServices;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Meshes;
using Surreal.Memory;

namespace Surreal.Internal.Graphics.Resources;

[DebuggerDisplay("Graphics buffer with {Length} elements ({Size})")]
internal sealed class OpenTKGraphicsBuffer<T> : GraphicsBuffer<T>
  where T : unmanaged
{
  private static readonly int Stride = Unsafe.SizeOf<T>();

  public BufferHandle Handle { get; } = GL.GenBuffer();

  public override unsafe Memory<T> Read(Optional<Range> range = default)
  {
    int sizeInBytes = 0;

    GL.BindBuffer(BufferTargetARB.ArrayBuffer, Handle);
    GL.GetBufferParameteri(BufferTargetARB.ArrayBuffer, BufferPNameARB.BufferSize, ref sizeInBytes);

    var (offset, length) = range.GetOrDefault(Range.All).GetOffsetAndLength(sizeInBytes / Stride);

    var offsetInBytes = offset * Stride;
    var buffer        = new T[length];

    fixed (T* raw = buffer)
    {
      GL.GetBufferSubData(
        target: BufferTargetARB.ArrayBuffer,
        offset: new IntPtr(offsetInBytes),
        size: sizeInBytes,
        data: raw
      );
    }

    return buffer;
  }

  public override unsafe void Write(ReadOnlySpan<T> buffer)
  {
    var bytes = buffer.Length * Stride;

    fixed (T* raw = buffer)
    {
      GL.BindBuffer(BufferTargetARB.ArrayBuffer, Handle);
      GL.BufferData(BufferTargetARB.ArrayBuffer, new Span<T>(raw, buffer.Length), BufferUsageARB.DynamicCopy);
    }

    Length = buffer.Length;
    Size   = new Size(bytes);
  }

  protected override void Dispose(bool managed)
  {
    GL.DeleteBuffer(Handle);

    base.Dispose(managed);
  }
}
