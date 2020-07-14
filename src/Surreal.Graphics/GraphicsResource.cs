using System;
using System.Collections.Generic;
using System.Linq;
using Surreal.Assets;
using Surreal.Graphics.Textures;
using Surreal.IO;

namespace Surreal.Graphics {
  public abstract class GraphicsResource : IDisposable {
    private static readonly List<GraphicsResource> Resources = new List<GraphicsResource>();

    public static Size AllocatedTextureSize => Resources.OfType<Texture>().Select(texture => texture.Size).Sum();
    public static Size TotalAllocatedSize   => Resources.OfType<IHasSizeEstimate>().Select(resource => resource.Size).Sum();

    private static void Track(GraphicsResource resource) {
      lock (Resources) {
        Resources.Add(resource);
      }
    }

    private static void Forget(GraphicsResource resource) {
      lock (Resources) {
        Resources.Remove(resource);
      }
    }

    protected GraphicsResource() {
      Track(this);
    }

    ~GraphicsResource() {
      Dispose(false);
    }

    public bool IsDisposed { get; private set; }

    public void Dispose() {
      if (!IsDisposed) {
        Dispose(true);
        IsDisposed = true;

        Forget(this);

        GC.SuppressFinalize(this);
      }
    }

    protected virtual void Dispose(bool managed) {
    }
  }
}