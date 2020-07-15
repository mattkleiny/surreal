using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Meshes;
using Surreal.IO;

namespace Surreal.Platform.Internal.Graphics.Resources {
  [DebuggerDisplay("Graphics buffer with {Length} elements ({Size})")]
  internal sealed class OpenTKGraphicsBuffer : GraphicsBuffer {
    public readonly int Id = GL.GenBuffer();

    public override Memory<T> Read<T>(Range range) {
      GL.BindBuffer(BufferTarget.ArrayBuffer, Id);
      GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int sizeInBytes);

      var sizeOfT = Unsafe.SizeOf<T>();

      var count = sizeInBytes / sizeOfT;
      var (offset, length) = range.GetOffsetAndLength(count);

      var offsetInBytes = offset * sizeOfT;

      // allocate enough space in the local heap. TODO: perhaps offer an overload where the caller can provide their own buffer?
      var buffer = new T[length];

      GL.GetBufferSubData(BufferTarget.CopyWriteBuffer, new IntPtr(offsetInBytes), new IntPtr(sizeInBytes), ref buffer[0]);

      return buffer;
    }

    public override void Write<T>(Span<T> data) {
      var bytes = data.Length * Unsafe.SizeOf<T>();

      GL.BindBuffer(BufferTarget.ArrayBuffer, Id);
      GL.BufferData(BufferTarget.ArrayBuffer, bytes, ref data.GetPinnableReference(), BufferUsageHint.StaticDraw);

      Length = data.Length;
      Size   = new Size(bytes);
    }

    public override MemoryManager<T> Pin<T>() {
      return new OpenTKMemoryManager<T>(Id, BufferTarget.ArrayBuffer);
    }

    protected override void Dispose(bool managed) {
      GL.DeleteBuffer(Id);

      base.Dispose(managed);
    }
  }
}