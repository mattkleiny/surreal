using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics;
using Surreal.Graphics.Materials.Shaders;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;
using Surreal.Platform.Internal.Graphics.Resources;
using Color = Surreal.Graphics.Color;
using Image = Surreal.Graphics.Textures.Image;
using PrimitiveType = Surreal.Graphics.Meshes.PrimitiveType;
using TextureWrapMode = Surreal.Graphics.Textures.TextureWrapMode;

namespace Surreal.Platform.Internal.Graphics {
  internal sealed class OpenTKGraphicsDevice : IGraphicsDevice, IDisposable {
    private readonly IDesktopWindow window;

    public OpenTKGraphicsDevice(IDesktopWindow window) {
      this.window = window;
    }

    public IPipelineState Pipeline { get; } = new OpenTKPipelineState();

    public void BeginFrame() {
    }

    public void Clear(Color color) {
      ClearDepth();
      ClearColor(color);
    }

    public void ClearColor(Color color) {
      GL.ClearColor(
          color.R / 255.0f,
          color.G / 255.0f,
          color.B / 255.0f,
          color.A / 255.0f
      );

      GL.Clear(ClearBufferMask.ColorBufferBit);
    }

    public void ClearDepth() {
      GL.Clear(ClearBufferMask.DepthBufferBit);
    }

    public void DrawMesh<TVertex>(
        Mesh<TVertex> mesh,
        ShaderProgram shader,
        int vertexCount,
        int indexCount,
        PrimitiveType type = PrimitiveType.Triangles)
        where TVertex : unmanaged {
      if (vertexCount == 0) return; // empty mesh? don't render

      Pipeline.ActiveShader       = shader;
      Pipeline.ActiveVertexBuffer = mesh.Vertices;
      Pipeline.ActiveIndexBuffer  = mesh.Indices;

      shader.Bind(mesh.Attributes);

      if (indexCount > 0) {
        DrawMeshIndexed(indexCount, type);
      }
      else {
        DrawMesh(vertexCount, type);
      }
    }

    public void Present() {
      window.Present();
    }

    public void EndFrame() {
      GL.Flush();
    }

    public GraphicsBuffer<T> CreateBuffer<T>() where T : unmanaged {
      return new OpenTKGraphicsBuffer<T>();
    }

    public ShaderProgram CreateShaderProgram(IReadOnlyList<Shader> shaders) {
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
      var image   = new Image(descriptor.Width, descriptor.Height);

      texture.Upload(image);

      return new OpenTKFrameBuffer(texture, image);
    }

    public void Dispose() {
    }

    private static void DrawMesh(int count, PrimitiveType type) {
      Debug.Assert(count >= 0, "count >= 0");

      GL.DrawArrays(ConvertPrimitiveType(type), 0, count);
    }

    private static void DrawMeshIndexed(int count, PrimitiveType type) {
      Debug.Assert(count >= 0, "count >= 0");

      GL.DrawElements(ConvertPrimitiveType(type), count, DrawElementsType.UnsignedShort, IntPtr.Zero);
    }

    private static OpenTK.Graphics.OpenGL.PrimitiveType ConvertPrimitiveType(PrimitiveType type) => type switch {
      PrimitiveType.Points    => OpenTK.Graphics.OpenGL.PrimitiveType.Points,
      PrimitiveType.Lines     => OpenTK.Graphics.OpenGL.PrimitiveType.Lines,
      PrimitiveType.LineStrip => OpenTK.Graphics.OpenGL.PrimitiveType.LineStrip,
      PrimitiveType.LineLoop  => OpenTK.Graphics.OpenGL.PrimitiveType.LineLoop,
      PrimitiveType.Triangles => OpenTK.Graphics.OpenGL.PrimitiveType.Triangles,
      PrimitiveType.Quads     => OpenTK.Graphics.OpenGL.PrimitiveType.Quads,
      PrimitiveType.QuadStrip => OpenTK.Graphics.OpenGL.PrimitiveType.QuadStrip,
      _                       => throw new ArgumentOutOfRangeException(nameof(type), type, "An unrecognized primitive type was requested."),
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
          _                                     => "An unexpected OpenGL error occurred.",
        });
      }
    }
  }
}