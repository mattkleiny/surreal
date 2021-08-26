using System;
using System.Collections.Generic;
using System.Linq;
using Surreal.Content;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Memory;

namespace Surreal.Graphics
{
  /// <summary>Base class for any graphical resource.</summary>
  public abstract class GraphicsResource : IDisposable
  {
    private static readonly List<GraphicsResource> Resources = new();

    public static Size AllocatedBufferSize  => SumSizeEstimates<GraphicsBuffer>();
    public static Size AllocatedTextureSize => SumSizeEstimates<Texture>();
    public static Size TotalAllocatedSize   => SumSizeEstimates<IHasSizeEstimate>();

    private static void Track(GraphicsResource resource)
    {
      lock (Resources)
      {
        Resources.Add(resource);
      }
    }

    private static void Forget(GraphicsResource resource)
    {
      lock (Resources)
      {
        Resources.Remove(resource);
      }
    }

    private static Size SumSizeEstimates<T>()
        where T : IHasSizeEstimate
    {
      lock (Resources)
      {
        return Resources.OfType<T>().Select(_ => _.Size).Sum();
      }
    }

    protected GraphicsResource()
    {
      Track(this);
    }

    ~GraphicsResource()
    {
      Dispose(false);
    }

    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
      if (!IsDisposed)
      {
        Dispose(true);
        IsDisposed = true;

        Forget(this);

        GC.SuppressFinalize(this);
      }
    }

    protected virtual void Dispose(bool managed)
    {
    }
  }
}
