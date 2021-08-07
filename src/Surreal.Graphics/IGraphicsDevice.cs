using System.Collections.Generic;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics
{
  public interface IGraphicsDevice
  {
    IPipelineState Pipeline { get; }

    Viewport Viewport
    {
      get => Pipeline.Rasterizer.Viewport;
      set => Pipeline.Rasterizer.Viewport = value;
    }

    void Clear(Color color);
    void ClearColor(Color color);
    void ClearDepth();

    void DrawMesh<TVertex>(
        Mesh<TVertex> mesh,
        MaterialPass pass,
        int vertexCount,
        int indexCount,
        PrimitiveType type = PrimitiveType.Triangles
    ) where TVertex : unmanaged;

    void BeginFrame();
    void EndFrame();
    void Present();

    GraphicsBuffer<T> CreateBuffer<T>() where T : unmanaged;
    ShaderProgram     CreateShaderProgram(params Shader[] shaders) => CreateShaderProgram(shaders as IReadOnlyList<Shader>);
    ShaderProgram     CreateShaderProgram(IReadOnlyList<Shader> shaders);

    Texture CreateTexture(
        TextureFormat format = TextureFormat.RGBA8888,
        TextureFilterMode filterMode = TextureFilterMode.Linear,
        TextureWrapMode wrapMode = TextureWrapMode.Repeat
    );

    Texture CreateTexture(
        ITextureData data,
        TextureFilterMode filterMode = TextureFilterMode.Linear,
        TextureWrapMode wrapMode = TextureWrapMode.Repeat
    );

    FrameBuffer CreateFrameBuffer(in FrameBufferDescriptor descriptor);
  }

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

  public interface IRasterizerState
  {
    Viewport Viewport { get; set; }

    bool IsDepthTestingEnabled { get; set; }
    bool IsBlendingEnabled     { get; set; }
  }

  public interface ITextureUnits
  {
    Texture? this[int unit] { get; set; }
  }
}