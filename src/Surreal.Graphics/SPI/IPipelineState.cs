using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.SPI.Rasterization;

namespace Surreal.Graphics.SPI
{
  public interface IPipelineState
  {
    FrameBuffer      PrimaryFrameBuffer { get; }
    FrameBuffer?     ActiveFrameBuffer  { get; set; }
    ShaderProgram?   ActiveShader       { get; set; }
    GraphicsBuffer?  ActiveVertexBuffer { get; set; }
    GraphicsBuffer?  ActiveIndexBuffer  { get; set; }
    ITextureUnits    TextureUnits       { get; }
    IRasterizerState Rasterizer         { get; }
  }
}