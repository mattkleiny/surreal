using System;
using System.Collections.Generic;
using System.Linq;
using Surreal.Assets;
using Surreal.Compute.Memory;
using Surreal.IO;

namespace Surreal.Compute {
  public abstract class ComputeResource : IDisposable {
    private static readonly IList<ComputeResource> Resources = new List<ComputeResource>();

    public static Size TotalBufferSize    => Resources.OfType<ComputeBuffer>().Select(_ => _.Size).Sum();
    public static Size TotalAllocatedSize => Resources.OfType<IHasSizeEstimate>().Select(_ => _.Size).Sum();

    private static void Track(ComputeResource resource) {
      lock (Resources) {
        Resources.Add(resource);
      }
    }

    private static void Forget(ComputeResource resource) {
      lock (Resources) {
        Resources.Remove(resource);
      }
    }

    protected ComputeResource() {
      Track(this);
    }

    ~ComputeResource() {
      Dispose(false);
    }

    public bool IsDisposed { get; private set; }

    public void Dispose() {
      if (!IsDisposed) {
        Dispose(true);
        IsDisposed = true;

        Forget(this);

        GC.SuppressFinalize(this);
      }
    }

    protected virtual void Dispose(bool managed) {
    }
  }
}