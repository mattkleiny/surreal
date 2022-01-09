using OpenTK.Graphics.OpenGL;
using Surreal.Compute.Execution;

namespace Surreal.Internal.Compute.Resources;

internal sealed class OpenTKComputeProgram : ComputeProgram
{
  public override void Execute(uint groupsX, uint groupsY, uint groupsZ)
  {
    GL.DispatchCompute(groupsX, groupsY, groupsZ);
    GL.MemoryBarrier(MemoryBarrierMask.ShaderImageAccessBarrierBit);
  }
}
