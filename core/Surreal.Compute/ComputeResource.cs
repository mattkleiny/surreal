using Surreal.Compute.Memory;
using Surreal.Memory;
using Surreal.Objects;

namespace Surreal.Compute;

/// <summary>A resource in the compute subsystem.</summary>
public abstract class ComputeResource : TrackedResource<ComputeResource>
{
  public static Size TotalBufferSize => GetSizeEstimate<ComputeBuffer>();
}
