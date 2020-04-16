using Surreal.Graphics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;

namespace Surreal.Platform.Internal.Graphics.Resources
{
  internal sealed class HeadlessCommandBuffer : CommandBuffer
  {
    public override void SetRenderTarget(FrameBuffer target)
    {
    }

    public override void ClearRenderTarget(Color color, bool clearColor, bool clearDepth)
    {
    }

    public override void DrawMesh(Mesh mesh, FrameBuffer source, FrameBuffer target, ShaderProgram shader)
    {
    }

    public override void Flush()
    {
    }
  }
}