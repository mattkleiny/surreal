using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;
using Surreal.Compute.Memory;
using Surreal.Data;

namespace Surreal.Platform.Internal.Compute.Resources {
  [DebuggerDisplay("Compute buffer with {Length} elements ({Size})")]
  internal sealed class OpenTKComputeBuffer<T> : ComputeBuffer<T>, IHasNativeId
      where T : unmanaged {
    private static readonly int Stride = Unsafe.SizeOf<T>();
    private readonly        int id     = GL.GenBuffer();

    int IHasNativeId.Id => id;

    public override Memory<T> Read(Range range) {
      GL.BindBuffer(BufferTarget.CopyWriteBuffer, id);
      GL.GetBufferParameter(BufferTarget.CopyWriteBuffer, BufferParameterName.BufferSize, out int sizeInBytes);

      var (offset, length) = range.GetOffsetAndLength(sizeInBytes / Stride);

      var offsetInBytes = offset * Stride;
      var buffer        = new T[length];

      GL.GetBufferSubData(BufferTarget.CopyWriteBuffer, new IntPtr(offsetInBytes), new IntPtr(sizeInBytes), ref buffer[0]);

      return buffer;
    }

    public override unsafe void Write(ReadOnlySpan<T> data) {
      var bytes = data.Length * Stride;

      fixed (T* raw = data) {
        GL.BindBuffer(BufferTarget.CopyWriteBuffer, id);
        GL.BufferData(BufferTarget.CopyWriteBuffer, bytes, ref Unsafe.AsRef<T>(raw), BufferUsageHint.DynamicCopy);
      }

      Length = data.Length;
      Size   = new Size(bytes);
    }

    protected override void Dispose(bool managed) {
      GL.DeleteBuffer(id);

      base.Dispose(managed);
    }
  }
}