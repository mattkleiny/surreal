using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.SPI;
using Surreal.Graphics.Textures;
using Surreal.Platform.Internal.Graphics.Resources;
using PrimitiveType = Surreal.Graphics.Meshes.PrimitiveType;
using TextureWrapMode = Surreal.Graphics.Textures.TextureWrapMode;

namespace Surreal.Platform.Internal.Graphics {
  internal sealed class OpenTKGraphicsBackend : IGraphicsBackend, IGraphicsFactory, IDisposable {
    public OpenTKGraphicsBackend(OpenTKWindow window) {
      SwapChain = new OpenTKSwapChain(window);
      Pipeline  = new OpenTKPipelineState();
    }

    public IGraphicsFactory Factory => this;

    public ISwapChain     SwapChain { get; }
    public IPipelineState Pipeline  { get; }

    public void BeginFrame() {
    }

    public void DrawMeshIndexed(int count, PrimitiveType type) {
      Check.That(count >= 0, "count >= 0");

      GL.DrawElements(ConvertPrimitiveType(type), count, DrawElementsType.UnsignedShort, IntPtr.Zero);
    }

    public void DrawMesh(int count, PrimitiveType type) {
      Check.That(count >= 0, "count >= 0");

      GL.DrawArrays(ConvertPrimitiveType(type), 0, count);
    }

    public void EndFrame() {
      GL.Flush();
    }

    public CommandBuffer CreateCommandBuffer() {
      return new OpenTKCommandBuffer(this);
    }

    public GraphicsBuffer CreateBuffer(int stride) {
      return new OpenTKGraphicsBuffer(stride);
    }

    public ShaderProgram CreateShaderProgram(params Shader[] shaders) {
      return new OpenTKShaderProgram(shaders);
    }

    public Texture CreateTexture(TextureFormat format, TextureFilterMode filterMode, TextureWrapMode wrapMode) {
      return new OpenTKTexture(format, filterMode, wrapMode);
    }

    public Texture CreateTexture(ITextureData data, TextureFilterMode filterMode, TextureWrapMode wrapMode) {
      return new OpenTKTexture(data, filterMode, wrapMode);
    }

    public FrameBuffer CreateFrameBuffer(in FrameBufferDescriptor descriptor) {
      var texture = (OpenTKTexture) CreateTexture(descriptor.Format, descriptor.FilterMode, TextureWrapMode.Clamp);
      var pixmap  = new Pixmap(descriptor.Width, descriptor.Height);

      texture.Upload(pixmap);

      return new OpenTKFrameBuffer(texture, pixmap);
    }

    public void Dispose() {
    }

    private static OpenTK.Graphics.OpenGL.PrimitiveType ConvertPrimitiveType(PrimitiveType type) => type switch {
        PrimitiveType.Points    => OpenTK.Graphics.OpenGL.PrimitiveType.Points,
        PrimitiveType.Lines     => OpenTK.Graphics.OpenGL.PrimitiveType.Lines,
        PrimitiveType.LineStrip => OpenTK.Graphics.OpenGL.PrimitiveType.LineStrip,
        PrimitiveType.LineLoop  => OpenTK.Graphics.OpenGL.PrimitiveType.LineLoop,
        PrimitiveType.Triangles => OpenTK.Graphics.OpenGL.PrimitiveType.Triangles,
        PrimitiveType.Quads     => OpenTK.Graphics.OpenGL.PrimitiveType.Quads,
        PrimitiveType.QuadStrip => OpenTK.Graphics.OpenGL.PrimitiveType.QuadStrip,
        _                       => throw new ArgumentOutOfRangeException(nameof(type), type, "An unrecognized primitive type was requested.")
    };

    [Conditional("DEBUG")]
    public static void CheckForErrors() {
      var errorCode = GL.GetError();
      if (errorCode != ErrorCode.NoError) {
        throw new PlatformException(errorCode switch {
            ErrorCode.InvalidEnum                 => "An invalid OpenGL enum was passed.",
            ErrorCode.InvalidValue                => "An invalid OpenGL value was passed.",
            ErrorCode.InvalidOperation            => "An invalid OpenGL operation was attempted.",
            ErrorCode.StackOverflow               => "The OpenGL stack has overflowed.",
            ErrorCode.StackUnderflow              => "The OpenGL stack has underflowed.",
            ErrorCode.OutOfMemory                 => "OpenGL is out of memory.",
            ErrorCode.InvalidFramebufferOperation => "An invalid OpenGL frame buffer operation was attempted.",
            ErrorCode.ContextLost                 => "The OpenGL context was lost.",
            ErrorCode.TableTooLarge               => "The OpenGL table is too large.",
            ErrorCode.TextureTooLargeExt          => "The OpenGL texture is too large.",
            _                                     => "An unexpected OpenGL error occurred."
        });
      }
    }
  }
}