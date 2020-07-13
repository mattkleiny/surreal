using Surreal.Graphics.Experimental.Rendering;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.SPI {
  public interface IPipelineState {
    FrameBuffer      PrimaryFrameBuffer { get; }
    FrameBuffer?     ActiveFrameBuffer  { get; set; }
    ShaderProgram?   ActiveShader       { get; set; }
    GraphicsBuffer?  ActiveVertexBuffer { get; set; }
    GraphicsBuffer?  ActiveIndexBuffer  { get; set; }
    ITextureUnits    TextureUnits       { get; }
    IRasterizerState Rasterizer         { get; }
  }

  public interface ITextureUnits {
    Texture? this[int unit] { get; set; }
  }

  public interface IRasterizerState {
    Viewport Viewport { get; set; }

    bool IsDepthTestingEnabled { get; set; }
    bool IsBlendingEnabled     { get; set; }
  }
}