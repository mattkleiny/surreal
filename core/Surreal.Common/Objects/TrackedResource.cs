using Surreal.Collections;
using Surreal.Memory;

namespace Surreal.Objects;

#pragma warning disable S2743
#pragma warning disable CA1000

/// <summary>A <see cref="Resource"/> with global tracking in a static <see cref="InterlinkedList{TNode}"/>.</summary>
public abstract class TrackedResource<TSelf> : Resource, IInterlinkedElement<TSelf>
  where TSelf : TrackedResource<TSelf>
{
  private static readonly InterlinkedList<TSelf> All = new();

  protected TrackedResource()
  {
    Track((TSelf) this);
  }

  public static Size TotalAllocatedSize => GetSizeEstimate<IHasSizeEstimate>();

  TSelf? IInterlinkedElement<TSelf>.Previous { get; set; }
  TSelf? IInterlinkedElement<TSelf>.Next     { get; set; }

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

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      Forget((TSelf) this);
    }

    base.Dispose(managed);
  }
}
