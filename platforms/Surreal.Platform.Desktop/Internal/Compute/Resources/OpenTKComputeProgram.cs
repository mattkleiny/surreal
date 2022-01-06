using OpenTK.Graphics.OpenGL;
using Surreal.Compute.Execution;
using Surreal.Internal.Graphics.Resources;

namespace Surreal.Internal.Compute.Resources;

internal sealed class OpenTKComputeProgram : ComputeProgram
{
  private readonly OpenTKShaderProgram computeShader;

  public OpenTKComputeProgram(ReadOnlySpan<byte> raw)
  {
    throw new NotImplementedException();
  }

  public override void Execute(uint groupsX, uint groupsY, uint groupsZ)
  {
    computeShader.Bind();

    GL.DispatchCompute(groupsX, groupsY, groupsZ);
    GL.MemoryBarrier(MemoryBarrierMask.ShaderImageAccessBarrierBit);
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      computeShader.Dispose();
    }

    base.Dispose(managed);
  }
}
