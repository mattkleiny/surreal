using System;
using Surreal.IO;

namespace Surreal.Graphics.Meshes {
  public abstract class GraphicsBuffer : GraphicsResource, IHardwareBuffer {
    public int Length { get; protected set; }

    public abstract Span<T> Read<T>(Range range)
        where T : unmanaged;

    public abstract void Write<T>(Span<T> data)
        where T : unmanaged;
  }
}