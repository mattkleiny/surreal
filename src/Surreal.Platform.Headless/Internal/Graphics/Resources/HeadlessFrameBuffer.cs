using Surreal.Graphics.Textures;

namespace Surreal.Platform.Internal.Graphics.Resources
{
  internal sealed class HeadlessFrameBuffer : FrameBuffer
  {
    public override Texture Texture { get; } = new HeadlessTexture(
        format: TextureFormat.RGBA8888,
        filterMode: TextureFilterMode.Point,
        wrapMode: TextureWrapMode.Clamp
    );
  }
}
