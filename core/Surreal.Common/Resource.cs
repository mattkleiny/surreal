using Surreal.Collections;
using Surreal.Memory;

namespace Surreal;

/// <summary>An application resource that can be deterministically destroyed.</summary>
public abstract class Resource : IDisposable
{
  ~Resource()
  {
    Dispose(false);
  }

  public bool IsDisposed { get; private set; }

  public void Dispose()
  {
    if (!IsDisposed)
    {
      Dispose(true);
      GC.SuppressFinalize(this);

      IsDisposed = true;
    }
  }

  protected virtual void Dispose(bool managed)
  {
  }
}

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
