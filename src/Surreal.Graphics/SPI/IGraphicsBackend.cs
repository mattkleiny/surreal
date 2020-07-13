using Surreal.Graphics.Experimental.Rendering;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.SPI {
  public interface IGraphicsBackend {
    IPipelineState Pipeline { get; }

    void BeginFrame();
    void ClearColorBuffer(Color color);
    void ClearDepthBuffer();
    void DrawMeshIndexed(int count, PrimitiveType type);
    void DrawMesh(int count, PrimitiveType type);
    void EndFrame();
    void Present();

    CommandBuffer  CreateCommandBuffer();
    GraphicsBuffer CreateBuffer(int stride);
    ShaderProgram  CreateShaderProgram(params Shader[] shaders);

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