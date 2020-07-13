using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;

namespace Surreal.Graphics.Rendering {
  public abstract class CommandBuffer : GraphicsResource {
    public abstract void SetRenderTarget(FrameBuffer target);
    public abstract void ClearRenderTarget(Color color, bool clearColor, bool clearDepth);

    public abstract void DrawMesh(Mesh mesh, FrameBuffer source, FrameBuffer target, ShaderProgram shader);

    public abstract void Flush();
  }
}