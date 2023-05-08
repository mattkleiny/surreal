using Surreal.Collections;
using Surreal.Memory;

namespace Surreal;

/// <summary>
/// An application resource that can be deterministically destroyed.
/// </summary>
public abstract class Resource : IDisposable
{
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

  ~Resource()
  {
    Dispose(false);
  }

  protected virtual void Dispose(bool managed)
  {
  }
}

/// <summary>
/// A <see cref="Resource" /> with global tracking in a static <see cref="IntrusiveLinkedList{TNode}" />.
/// </summary>
public abstract class TrackedResource<TSelf> : Resource, IIntrusiveLinkedListNode<TSelf>
  where TSelf : TrackedResource<TSelf>
{
  private static readonly IntrusiveLinkedList<TSelf> All = new();

  protected TrackedResource()
  {
    Track((TSelf)this);
  }

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

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      Forget((TSelf)this);
    }

    base.Dispose(managed);
  }

  TSelf? IIntrusiveLinkedListNode<TSelf>.Previous { get; set; }
  TSelf? IIntrusiveLinkedListNode<TSelf>.Next { get; set; }
}
