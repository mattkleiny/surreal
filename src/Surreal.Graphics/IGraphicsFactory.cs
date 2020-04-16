using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics
{
  public interface IGraphicsFactory
  {
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