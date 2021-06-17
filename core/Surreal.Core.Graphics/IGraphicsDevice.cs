using System.Collections.Generic;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics {
  public interface IGraphicsDevice {
    IPipelineState Pipeline { get; }

    Viewport Viewport {
      get => Pipeline.Rasterizer.Viewport;
      set => Pipeline.Rasterizer.Viewport = value;
    }

    void Clear(Color color);
    void ClearColor(Color color);
    void ClearDepth();

    void DrawMesh<TVertex>(Mesh<TVertex> mesh, ShaderProgram shader, int vertexCount, int indexCount, PrimitiveType type = PrimitiveType.Triangles)
        where TVertex : unmanaged;

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
}