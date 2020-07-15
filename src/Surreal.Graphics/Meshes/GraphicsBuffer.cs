using System;
using System.Buffers;
using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Graphics.Meshes {
  public abstract class GraphicsBuffer : GraphicsResource, IHasSizeEstimate {
    public int  Length { get; protected set; }
    public Size Size   { get; protected set; }

    public abstract Memory<T> Read<T>(Range range)
        where T : unmanaged;

    public abstract void Write<T>(Span<T> data)
        where T : unmanaged;

    public abstract MemoryManager<T> Pin<T>()
        where T : unmanaged;
  }
}