using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Meshes;

namespace Surreal.Platform.Internal.Graphics.Resources {
  [DebuggerDisplay("Graphics buffer with {Count} elements ~{Size}")]
  internal sealed class OpenTKGraphicsBuffer : GraphicsBuffer {
    public readonly int Id = GL.GenBuffer();

    public OpenTKGraphicsBuffer(int stride)
        : base(stride) {
    }

    public override void Put<T>(Span<T> data) {
      var bytes = data.Length * Unsafe.SizeOf<T>();

      GL.BindBuffer(BufferTarget.ArrayBuffer, Id);
      GL.BufferData(BufferTarget.ArrayBuffer, bytes, ref data.GetPinnableReference(), BufferUsageHint.StaticDraw);

      Count = data.Length;
    }

    protected override void Dispose(bool managed) {
      GL.DeleteBuffer(Id);

      base.Dispose(managed);
    }
  }
}