using System;
using System.Collections.Generic;
using System.Linq;
using Surreal.Assets;
using Surreal.Compute.Memory;
using Surreal.Memory;

namespace Surreal.Compute
{
  public abstract class ComputeResource : IDisposable
  {
    private static readonly List<ComputeResource> Resources = new();

    public static Size TotalBufferSize    => SumSizeEstimates<ComputeBuffer>();
    public static Size TotalAllocatedSize => SumSizeEstimates<IHasSizeEstimate>();

    private static void Track(ComputeResource resource)
    {
      lock (Resources)
      {
        Resources.Add(resource);
      }
    }

    private static void Forget(ComputeResource resource)
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

    protected ComputeResource()
    {
      Track(this);
    }

    ~ComputeResource()
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
