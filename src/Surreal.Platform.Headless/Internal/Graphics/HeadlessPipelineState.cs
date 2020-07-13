using Surreal.Graphics.Experimental.Rendering;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.SPI;
using Surreal.Platform.Internal.Graphics.Resources;

namespace Surreal.Platform.Internal.Graphics {
  internal sealed class HeadlessPipelineState : IPipelineState {
    public FrameBuffer     PrimaryFrameBuffer { get; } = new HeadlessFrameBuffer();
    public FrameBuffer?    ActiveFrameBuffer  { get; set; }
    public ShaderProgram?  ActiveShader       { get; set; }
    public GraphicsBuffer? ActiveVertexBuffer { get; set; }
    public GraphicsBuffer? ActiveIndexBuffer  { get; set; }

    public ITextureUnits    TextureUnits { get; } = new HeadlessTextureUnits();
    public IRasterizerState Rasterizer   { get; } = new HeadlessRasterizerState();
  }
}