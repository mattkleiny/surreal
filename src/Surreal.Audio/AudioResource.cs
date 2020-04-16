using System;
using System.Collections.Generic;
using System.Linq;
using Surreal.Assets;
using Surreal.Audio.Clips;
using Surreal.Memory;

namespace Surreal.Audio
{
  public abstract class AudioResource : IDisposable
  {
    private static readonly IList<AudioResource> Resources = new List<AudioResource>();

    public static Size AllocatedClipSize
    {
      get
      {
        lock (Resources)
        {
          return Resources.OfType<AudioClip>().Select(clip => clip.Size).Sum();
        }
      }
    }

    public static Size TotalAllocatedSize
    {
      get
      {
        lock (Resources)
        {
          return Resources.OfType<IHasSizeEstimate>().Select(resource => resource.Size).Sum();
        }
      }
    }

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
