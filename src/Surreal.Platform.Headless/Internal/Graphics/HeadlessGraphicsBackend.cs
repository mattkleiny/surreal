using Surreal.Graphics;
using Surreal.Graphics.Experimental.Rendering;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.SPI;
using Surreal.Graphics.Textures;
using Surreal.Platform.Internal.Graphics.Resources;

namespace Surreal.Platform.Internal.Graphics {
  internal sealed class HeadlessGraphicsBackend : IGraphicsBackend {
    public IPipelineState Pipeline { get; } = new HeadlessPipelineState();

    public void BeginFrame() {
      // no-op
    }

    public void ClearColorBuffer(Color color) {
      // no-op
    }

    public void ClearDepthBuffer() {
      // no-op
    }

    public void DrawMeshIndexed(int count, PrimitiveType type) {
      // no-op
    }

    public void DrawMesh(int count, PrimitiveType type) {
      // no-op
    }

    public void EndFrame() {
      // no-op
    }

    public void Present() {
      // no-op
    }

    public CommandBuffer CreateCommandBuffer() {
      return new HeadlessCommandBuffer();
    }

    public GraphicsBuffer CreateBuffer(int stride) {
      return new HeadlessGraphicsBuffer(stride);
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