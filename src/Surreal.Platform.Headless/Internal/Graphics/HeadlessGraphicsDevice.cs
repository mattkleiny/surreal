using Surreal.Graphics;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;
using Surreal.Platform.Internal.Graphics.Resources;

namespace Surreal.Platform.Internal.Graphics {
  internal sealed class HeadlessGraphicsDevice : IGraphicsDevice {
    public IPipelineState Pipeline { get; } = new HeadlessPipelineState();

    public void BeginFrame() {
      // no-op
    }

    public void Clear(Color color) {
      // no-op
    }

    public void ClearColor(Color color) {
      // no-op
    }

    public void ClearDepth() {
      // no-op
    }

    public void DrawMeshIndexed(int count, PrimitiveType type) {
      // no-op
    }

    public void DrawMesh(int count, PrimitiveType type) {
      // no-op
    }

    public void DrawMeshImmediate<TVertex>(
        Mesh<TVertex> mesh,
        ShaderProgram shader,
        int vertexCount,
        int indexCount,
        PrimitiveType type = PrimitiveType.Triangles)
        where TVertex : unmanaged {
      // no-op
    }

    public void EndFrame() {
      // no-op
    }

    public void Present() {
      // no-op
    }

    public GraphicsBuffer<T> CreateBuffer<T>() where T : unmanaged {
      return new HeadlessGraphicsBuffer<T>();
    }

    public ShaderProgram CreateShaderProgram(params Shader[] shaders) {
      return new HeadlessShaderProgram();
    }

    public Texture CreateTexture(TextureFormat format, TextureFilterMode filterMode, TextureWrapMode wrapMode) {
      return new HeadlessTexture(format, filterMode, wrapMode);
    }

    public Texture CreateTexture(ITextureData data, TextureFilterMode filterMode, TextureWrapMode wrapMode) {
      return new HeadlessTexture(data.Format, filterMode, wrapMode);
    }

    public FrameBuffer CreateFrameBuffer(in FrameBufferDescriptor descriptor) {
      return new HeadlessFrameBuffer();
    }
  }
}