using Surreal.Collections;
using Surreal.Memory;

namespace Surreal.Assets;

/// <summary>
/// An application asset that can be managed and unloaded.
/// </summary>
public abstract class Asset : IDisposable
{
  ~Asset()
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

/// <summary>
/// A <see cref="Asset" /> with global tracking in a static <see cref="IntrusiveLinkedList{TNode}" />.
/// </summary>
public abstract class TrackedAsset<TSelf> : Asset, IIntrusiveLinkedListNode<TSelf>
  where TSelf : TrackedAsset<TSelf>
{
  /// <summary>
  /// All of the <see cref="TSelf"/> assets.
  /// </summary>
  public static IntrusiveLinkedList<TSelf> All { get; } = new();

  /// <summary>
  /// The total size of all tracked assets.
  /// </summary>
  public static Size TotalAllocatedSize => GetSizeEstimate<IHasSizeEstimate>();

  /// <summary>
  /// Gets the total size of all tracked assets of the given type.
  /// </summary>
  public static Size GetSizeEstimate<T>()
    where T : IHasSizeEstimate
  {
    lock (All)
    {
      return All.OfType<T>().Select(x => x.Size).Sum();
    }
  }

  protected TrackedAsset()
  {
    Track((TSelf)this);
  }

  private static void Track(TSelf asset)
  {
    lock (All)
    {
      All.Add(asset);
    }
  }

  private static void Forget(TSelf asset)
  {
    lock (All)
    {
      All.Remove(asset);
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
