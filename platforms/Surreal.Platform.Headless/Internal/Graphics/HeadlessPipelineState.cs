using Surreal.Graphics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Internal.Graphics.Resources;

namespace Surreal.Internal.Graphics;

internal sealed class HeadlessPipelineState : IPipelineState
{
  public RenderTexture     PrimaryRenderTexture { get; } = new HeadlessRenderTexture();
  public RenderTexture?    ActiveFrameBuffer  { get; set; }
  public ShaderProgram?  ActiveShader       { get; set; }
  public GraphicsBuffer? ActiveVertexBuffer { get; set; }
  public GraphicsBuffer? ActiveIndexBuffer  { get; set; }

  public ITextureUnits    TextureUnits { get; } = new HeadlessTextureUnits();
  public IRasterizerState Rasterizer   { get; } = new HeadlessRasterizerState();
}
