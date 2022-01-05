using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Meshes;
using Surreal.Memory;

namespace Surreal.Internal.Graphics.Resources;

[DebuggerDisplay("Graphics buffer with {Length} elements ({Size})")]
internal sealed class OpenTkGraphicsBuffer<T> : GraphicsBuffer<T>, IHasNativeId
  where T : unmanaged
{
  private static readonly int Stride = Unsafe.SizeOf<T>();

  public int Id { get; } = GL.GenBuffer();

  public override Memory<T> Read(Optional<Range> range = default)
  {
    GL.BindBuffer(BufferTarget.ArrayBuffer, Id);
    GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int sizeInBytes);

    var (offset, length) = range.GetOrDefault(Range.All).GetOffsetAndLength(sizeInBytes / Stride);

    var offsetInBytes = offset * Stride;
    var buffer        = new T[length];

    GL.GetBufferSubData(
      target: BufferTarget.CopyWriteBuffer,
      offset: new IntPtr(offsetInBytes),
      size: new IntPtr(sizeInBytes),
      data: ref buffer[0]
    );

    return buffer;
  }

  public override unsafe void Write(ReadOnlySpan<T> data)
  {
    var bytes = data.Length * Stride;

    fixed (T* raw = data)
    {
      GL.BindBuffer(BufferTarget.ArrayBuffer, Id);
      GL.BufferData(BufferTarget.ArrayBuffer, bytes, ref Unsafe.AsRef<T>(raw), BufferUsageHint.DynamicCopy);
    }

    Length = data.Length;
    Size   = new Size(bytes);
  }

  protected override void Dispose(bool managed)
  {
    GL.DeleteBuffer(Id);

    base.Dispose(managed);
  }
}
