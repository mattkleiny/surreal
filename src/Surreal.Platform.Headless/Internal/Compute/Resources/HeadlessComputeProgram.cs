using Surreal.Compute.Execution;

namespace Surreal.Platform.Internal.Compute.Resources
{
  internal sealed class HeadlessComputeProgram : ComputeProgram
  {
    public override void Execute(int groupsX, int groupsY, int groupsZ)
    {
      // no-op
    }
  }
}
