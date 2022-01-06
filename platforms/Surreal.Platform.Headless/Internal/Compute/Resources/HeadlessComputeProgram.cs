using Surreal.Compute.Execution;

namespace Surreal.Internal.Compute.Resources;

internal sealed class HeadlessComputeProgram : ComputeProgram
{
  public override void Execute(uint groupsX, uint groupsY, uint groupsZ)
  {
    // no-op
  }
}
