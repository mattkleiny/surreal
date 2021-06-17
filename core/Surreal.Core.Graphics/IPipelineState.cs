using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;

namespace Surreal.Graphics {
  public interface IPipelineState {
    FrameBuffer      PrimaryFrameBuffer { get; }
    FrameBuffer?     ActiveFrameBuffer  { get; set; }
    ShaderProgram?   ActiveShader       { get; set; }
    GraphicsBuffer?  ActiveVertexBuffer { get; set; }
    GraphicsBuffer?  ActiveIndexBuffer  { get; set; }
    ITextureUnits    TextureUnits       { get; }
    IRasterizerState Rasterizer         { get; }
  }
}