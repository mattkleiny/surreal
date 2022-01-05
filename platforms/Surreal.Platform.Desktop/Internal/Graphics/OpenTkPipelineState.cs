using OpenTK.Graphics.OpenGL;
using Surreal.Graphics;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Internal.Graphics.Resources;

namespace Surreal.Internal.Graphics;

internal sealed class OpenTkPipelineState : IPipelineState
{
  private IHasNativeId? activeFrameBuffer;
  private IHasNativeId? activeShader;
  private IHasNativeId? activeVertexBuffer;
  private IHasNativeId? activeIndexBuffer;

  public RenderTexture? ActiveFrameBuffer
  {
    get => (RenderTexture?) activeFrameBuffer;
    set
    {
      activeFrameBuffer = (OpenTkRenderTexture?) value;
      GL.BindFramebuffer(FramebufferTarget.Framebuffer, activeFrameBuffer?.Id ?? 0);
    }
  }

  public ShaderProgram? ActiveShader
  {
    get => (ShaderProgram?) activeShader;
    set
    {
      activeShader = (OpenTkShaderProgram?) value;
      GL.UseProgram(activeShader?.Id ?? 0);
    }
  }

  public GraphicsBuffer? ActiveVertexBuffer
  {
    get => (GraphicsBuffer?) activeVertexBuffer;
    set
    {
      activeVertexBuffer = (IHasNativeId?) value;
      GL.BindBuffer(BufferTarget.ArrayBuffer, activeVertexBuffer?.Id ?? 0);
    }
  }

  public GraphicsBuffer? ActiveIndexBuffer
  {
    get => (GraphicsBuffer?) activeIndexBuffer;
    set
    {
      activeIndexBuffer = (IHasNativeId?) value;
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, activeIndexBuffer?.Id ?? 0);
    }
  }

  public ITextureUnits    TextureUnits { get; } = new OpenTkTextureUnits(capacity: 10);
  public IRasterizerState Rasterizer   { get; } = new OpenTkRasterizerState();
}
