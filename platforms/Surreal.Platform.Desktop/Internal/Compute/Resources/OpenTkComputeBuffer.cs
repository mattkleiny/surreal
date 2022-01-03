using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;
using Surreal.Compute.Memory;
using Surreal.Memory;

namespace Surreal.Internal.Compute.Resources;

[DebuggerDisplay("Compute buffer with {Length} elements ({Size})")]
internal sealed class OpenTkComputeBuffer<T> : ComputeBuffer<T>, IHasNativeId
  where T : unmanaged
{
  private static readonly int Stride = Unsafe.SizeOf<T>();

  public int Id { get; } = GL.GenBuffer();

  public override Memory<T> Read(Optional<Range> range = default)
  {
    GL.BindBuffer(BufferTarget.CopyWriteBuffer, Id);
    GL.GetBufferParameter(BufferTarget.CopyWriteBuffer, BufferParameterName.BufferSize, out int sizeInBytes);

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
      GL.BindBuffer(BufferTarget.CopyWriteBuffer, Id);
      GL.BufferData(BufferTarget.CopyWriteBuffer, bytes, ref Unsafe.AsRef<T>(raw), BufferUsageHint.DynamicCopy);
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
