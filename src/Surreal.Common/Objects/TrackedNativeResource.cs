using System.Linq;
using Surreal.Collections;
using Surreal.Content;
using Surreal.Memory;

namespace Surreal.Objects
{
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
