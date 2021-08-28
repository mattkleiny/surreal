using OpenTK.Graphics.OpenGL;
using Surreal.Graphics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Platform.Internal.Graphics.Resources;

namespace Surreal.Platform.Internal.Graphics
{
  internal sealed class OpenTKPipelineState : IPipelineState
  {
    private IHasNativeId? activeFrameBuffer;
    private IHasNativeId? activeShader;
    private IHasNativeId? activeVertexBuffer;
    private IHasNativeId? activeIndexBuffer;

    public FrameBuffer PrimaryFrameBuffer { get; } = new OpenTKPrimaryFrameBuffer();

    public FrameBuffer? ActiveFrameBuffer
    {
      get => (FrameBuffer?)activeFrameBuffer;
      set
      {
        activeFrameBuffer = (OpenTKFrameBuffer?)value;
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, activeFrameBuffer?.Id ?? 0);
      }
    }

    public ShaderProgram? ActiveShader
    {
      get => (ShaderProgram?)activeShader;
      set
      {
        activeShader = (OpenTKShaderProgram?)value;
        GL.UseProgram(activeShader?.Id ?? 0);
      }
    }

    public GraphicsBuffer? ActiveVertexBuffer
    {
      get => (GraphicsBuffer?)activeVertexBuffer;
      set
      {
        activeVertexBuffer = (IHasNativeId?)value;
        GL.BindBuffer(BufferTarget.ArrayBuffer, activeVertexBuffer?.Id ?? 0);
      }
    }

    public GraphicsBuffer? ActiveIndexBuffer
    {
      get => (GraphicsBuffer?)activeIndexBuffer;
      set
      {
        activeIndexBuffer = (IHasNativeId?)value;
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, activeIndexBuffer?.Id ?? 0);
      }
    }

    public ITextureUnits    TextureUnits { get; } = new OpenTKTextureUnits(capacity: 10);
    public IRasterizerState Rasterizer   { get; } = new OpenTKRasterizerState();
  }
}
