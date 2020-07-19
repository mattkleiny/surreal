using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;
using Surreal.Compute.Memory;
using Surreal.IO;

namespace Surreal.Platform.Internal.Compute.Resources {
  [DebuggerDisplay("Compute buffer with {Length} elements ({Size})")]
  internal sealed class OpenTKComputeBuffer : ComputeBuffer {
    public readonly int Id = GL.GenBuffer();

    public override Memory<T> Read<T>(Range range) {
      GL.BindBuffer(BufferTarget.CopyWriteBuffer, Id);
      GL.GetBufferParameter(BufferTarget.CopyWriteBuffer, BufferParameterName.BufferSize, out int sizeInBytes);

      var sizeOfT = Unsafe.SizeOf<T>();

      var count = sizeInBytes / sizeOfT;
      var (offset, length) = range.GetOffsetAndLength(count);

      var offsetInBytes = offset * sizeOfT;

      // allocate enough space in the local heap.
      // TODO: perhaps offer an overload where the caller can provide their own buffer?
      var buffer = new T[length];

      GL.GetBufferSubData(BufferTarget.CopyWriteBuffer, new IntPtr(offsetInBytes), new IntPtr(sizeInBytes), ref buffer[0]);

      return buffer;
    }

    public override void Write<T>(Span<T> data) {
      var bytes = data.Length * Unsafe.SizeOf<T>();

      GL.BindBuffer(BufferTarget.CopyWriteBuffer, Id);
      GL.BufferData(BufferTarget.CopyWriteBuffer, bytes, ref data.GetPinnableReference(), BufferUsageHint.DynamicCopy);

      Length = data.Length;
      Size   = new Size(bytes);
    }

    protected override void Dispose(bool managed) {
      GL.DeleteBuffer(Id);

      base.Dispose(managed);
    }
  }
}