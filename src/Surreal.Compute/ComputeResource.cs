using Surreal.Compute.Memory;
using Surreal.Memory;
using Surreal.Objects;

namespace Surreal.Compute
{
  public abstract class ComputeResource : TrackedNativeResource<ComputeResource>
  {
    public static Size TotalBufferSize => GetSizeEstimate<ComputeBuffer>();
  }
}
