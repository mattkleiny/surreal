using System;
using System.Linq;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.IO;
using Surreal.Memory;

namespace Surreal.Objects
{
  /// <summary>An application resource that can be deterministically destroyed.</summary>
  public abstract class Resource : IDisposable
  {
    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
      if (!IsDisposed)
      {
        Dispose(true);
        IsDisposed = true;
      }
    }

    protected virtual void Dispose(bool managed)
    {
    }
  }

  /// <summary>A <see cref="Resource"/> with native underlying data that must be finalised.</summary>
  public abstract class NativeResource : Resource
  {
    ~NativeResource()
    {
      Dispose(false);
    }

    protected override void Dispose(bool managed)
    {
      GC.SuppressFinalize(this);

      base.Dispose(managed);
    }
  }

  /// <summary>A <see cref="NativeResource"/> with global tracking in a static <see cref="LinkedNodeList{TNode}"/>.</summary>
  public abstract class TrackedNativeResource<TSelf> : NativeResource, ILinkedElement<TSelf>
      where TSelf : TrackedNativeResource<TSelf>
  {
    private static readonly LinkedNodeList<TSelf> Resources = new();

    public static Size TotalAllocatedSize => GetSizeEstimate<IHasSizeEstimate>();

    public static Size GetSizeEstimate<T>()
        where T : IHasSizeEstimate
    {
      lock (Resources)
      {
        return Resources.OfType<T>().Select(_ => _.Size).Sum();
      }
    }

    private static void Track(TSelf resource)
    {
      lock (Resources)
      {
        Resources.Add(resource);
      }
    }

    private static void Forget(TSelf resource)
    {
      lock (Resources)
      {
        Resources.Remove(resource);
      }
    }

    protected TrackedNativeResource()
    {
      Track((TSelf) this);
    }

    protected override void Dispose(bool managed)
    {
      if (managed)
      {
        Forget((TSelf) this);
      }

      base.Dispose(managed);
    }

    TSelf? ILinkedElement<TSelf>.Previous { get; set; }
    TSelf? ILinkedElement<TSelf>.Next     { get; set; }
  }
}
