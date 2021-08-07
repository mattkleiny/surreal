using System;
using Surreal.Assets;
using Surreal.Memory;

namespace Surreal.Graphics.Meshes
{
  public abstract class GraphicsBuffer : GraphicsResource, IHasSizeEstimate
  {
    public int  Length { get; protected set; }
    public Size Size   { get; protected set; }
  }

  public abstract class GraphicsBuffer<T> : GraphicsBuffer
      where T : unmanaged
  {
    public          Memory<T> Read() => Read(Range.All);
    public abstract Memory<T> Read(Range range);
    public abstract void      Write(ReadOnlySpan<T> data);
  }
}