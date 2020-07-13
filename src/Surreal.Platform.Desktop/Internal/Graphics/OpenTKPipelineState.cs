using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.SPI;
using Surreal.Graphics.SPI.Rasterization;
using Surreal.Platform.Internal.Graphics.Resources;

namespace Surreal.Platform.Internal.Graphics {
  internal sealed class OpenTKPipelineState : IPipelineState {
    private OpenTKFrameBuffer?    activeFrameBuffer;
    private OpenTKShaderProgram?  activeShader;
    private OpenTKGraphicsBuffer? activeVertexBuffer;
    private OpenTKGraphicsBuffer? activeIndexBuffer;

    public FrameBuffer PrimaryFrameBuffer { get; } = new OpenTKPrimaryFrameBuffer();

    public FrameBuffer? ActiveFrameBuffer {
      get => activeFrameBuffer;
      set {
        activeFrameBuffer = (OpenTKFrameBuffer?) value;
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, activeFrameBuffer?.Id ?? 0);
      }
    }

    public ShaderProgram? ActiveShader {
      get => activeShader;
      set {
        activeShader = (OpenTKShaderProgram?) value;
        GL.UseProgram(activeShader?.Id ?? 0);
      }
    }

    public GraphicsBuffer? ActiveVertexBuffer {
      get => activeVertexBuffer;
      set {
        activeVertexBuffer = (OpenTKGraphicsBuffer?) value;
        GL.BindBuffer(BufferTarget.ArrayBuffer, activeVertexBuffer?.Id ?? 0);
      }
    }

    public GraphicsBuffer? ActiveIndexBuffer {
      get => activeIndexBuffer;
      set {
        activeIndexBuffer = (OpenTKGraphicsBuffer?) value;
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, activeIndexBuffer?.Id ?? 0);
      }
    }

    public ITextureUnits    TextureUnits { get; } = new OpenTKTextureUnits(capacity: 10);
    public IRasterizerState Rasterizer   { get; } = new OpenTKRasterizerState();
  }
}