using Surreal.Collections;
using Surreal.Memory;

namespace Surreal.Objects;

/// <summary>A <see cref="NativeResource"/> with global tracking in a static <see cref="LinkedNodeList{TNode}"/>.</summary>
public abstract class TrackedNativeResource<TSelf> : NativeResource, ILinkedElement<TSelf>
  where TSelf : TrackedNativeResource<TSelf>
{
  private static readonly LinkedNodeList<TSelf> All = new();

  public static Size TotalAllocatedSize => GetSizeEstimate<IHasSizeEstimate>();

  public static Size GetSizeEstimate<T>()
    where T : IHasSizeEstimate
  {
    lock (All)
    {
      return All.OfType<T>().Select(_ => _.Size).Sum();
    }
  }

  private static void Track(TSelf resource)
  {
    lock (All)
    {
      All.Add(resource);
    }
  }

  private static void Forget(TSelf resource)
  {
    lock (All)
    {
      All.Remove(resource);
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