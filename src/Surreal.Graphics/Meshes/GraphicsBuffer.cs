using System;
using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Graphics.Meshes {
  public abstract class GraphicsBuffer : GraphicsResource, IHasSizeEstimate {
    protected GraphicsBuffer(int stride) {
      Check.That(stride > 0, "stride > 0");

      Stride = stride;
    }

    public int  Stride { get; }
    public int  Count  { get; protected set; }
    public Size Size   => new Size(Stride * Count);

    public abstract void Put<T>(Span<T> data)
        where T : unmanaged;
  }
}