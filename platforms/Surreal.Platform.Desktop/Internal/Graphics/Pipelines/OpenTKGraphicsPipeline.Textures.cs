using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Pipelines;
using Surreal.Graphics.Textures;
using static Surreal.Graphics.Pipelines.IGraphicsPipeline;

namespace Surreal.Internal.Graphics.Pipelines;

public sealed partial class OpenTKGraphicsPipeline : ITextures
{
  public ITextures Textures => this;

  public GraphicsId CreateTexture()
  {
    var texture = GL.CreateTexture(TextureTarget.Texture2d);

    return new GraphicsId(texture.Handle);
  }

  public void Allocate(GraphicsId id, int width, int height, int depth, TextureFormat format)
  {
    var handle = new TextureHandle(id);

    GL.TextureParameterf(handle, TextureParameterName.TextureWidth, width);
    GL.TextureParameterf(handle, TextureParameterName.TextureHeight, height);

    if (depth > 0)
    {
      GL.TextureParameterf(handle, TextureParameterName.TextureDepthExt, depth);
    }
  }

  public void UploadTexels<T>(GraphicsId id, ReadOnlySpan<T> data, int mipLevel = 0)
  {
    throw new NotImplementedException();
  }
}
