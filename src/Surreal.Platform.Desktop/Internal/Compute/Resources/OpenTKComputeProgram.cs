using System;
using OpenTK.Graphics.OpenGL;
using Surreal.Compute.Execution;
using Surreal.Graphics.Materials;
using Surreal.Platform.Internal.Graphics.Resources;
using ShaderType = Surreal.Graphics.Materials.ShaderType;

namespace Surreal.Platform.Internal.Compute.Resources {
  internal sealed class OpenTKComputeProgram : ComputeProgram {
    private readonly OpenTKShaderProgram computeShader;

    public OpenTKComputeProgram(ReadOnlySpan<byte> raw) {
      var shader = new Shader(ShaderType.Compute, raw.ToArray());

      computeShader = new OpenTKShaderProgram(shader);
    }

    public override void Execute(int groupsX, int groupsY, int groupsZ) {
      Check.That(groupsX >= 0, "groupsX >= 0");
      Check.That(groupsY >= 0, "groupsY >= 0");
      Check.That(groupsZ >= 0, "groupsZ >= 0");

      computeShader.Bind();

      GL.DispatchCompute(groupsX, groupsY, groupsZ);
      GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);
    }

    protected override void Dispose(bool managed) {
      if (managed) {
        computeShader.Dispose();
      }

      base.Dispose(managed);
    }
  }
}