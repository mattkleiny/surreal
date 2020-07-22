using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Meshes;
using Surreal.IO;

namespace Surreal.Platform.Internal.Graphics.Resources {
  [DebuggerDisplay("Graphics buffer with {Length} elements ({Size})")]
  internal sealed class OpenTKGraphicsBuffer<T> : GraphicsBuffer<T>, IHasNativeId
      where T : unmanaged {
    private static readonly int Stride = Unsafe.SizeOf<T>();
    private readonly        int Id     = GL.GenBuffer();

    int IHasNativeId.Id => Id;

    public override Memory<T> Read(Range range) {
      GL.BindBuffer(BufferTarget.ArrayBuffer, Id);
      GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int sizeInBytes);

      var count = sizeInBytes / Stride;
      var (offset, length) = range.GetOffsetAndLength(count);

      var offsetInBytes = offset * Stride;

      // allocate enough space in the local heap. TODO: perhaps offer an overload where the caller can provide their own buffer?
      var buffer = new T[length];

      GL.GetBufferSubData(BufferTarget.CopyWriteBuffer, new IntPtr(offsetInBytes), new IntPtr(sizeInBytes), ref buffer[0]);

      return buffer;
    }

    public override unsafe void Write(ReadOnlySpan<T> data) {
      var bytes = data.Length * Stride;

      fixed (T* raw = data) {
        GL.BindBuffer(BufferTarget.ArrayBuffer, Id);
        GL.BufferData(BufferTarget.ArrayBuffer, bytes, ref Unsafe.AsRef<T>(raw), BufferUsageHint.DynamicCopy);
      }

      Length = data.Length;
      Size   = new Size(bytes);
    }

    protected override void Dispose(bool managed) {
      GL.DeleteBuffer(Id);

      base.Dispose(managed);
    }
  }
}