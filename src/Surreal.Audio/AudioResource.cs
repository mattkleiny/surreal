using System;
using System.Collections.Generic;
using System.Linq;
using Surreal.Assets;
using Surreal.Audio.Clips;
using Surreal.Memory;

namespace Surreal.Audio
{
  /// <summary>Base class for any audio resource.</summary>
  public abstract class AudioResource : IDisposable
  {
    private static readonly List<AudioResource> Resources = new();

    public static Size AllocatedClipSize  => SumSizeEstimates<AudioClip>();
    public static Size TotalAllocatedSize => SumSizeEstimates<IHasSizeEstimate>();

    private static void Track(AudioResource resource)
    {
      lock (Resources)
      {
        Resources.Add(resource);
      }
    }

    private static void Forget(AudioResource resource)
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

    protected AudioResource()
    {
      Track(this);
    }

    ~AudioResource()
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
