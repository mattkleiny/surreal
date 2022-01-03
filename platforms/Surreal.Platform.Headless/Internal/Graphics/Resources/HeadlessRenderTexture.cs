using Surreal.Graphics.Textures;

namespace Surreal.Internal.Graphics.Resources;

internal sealed class HeadlessRenderTexture : RenderTexture
{
  public override Texture Texture { get; } = new HeadlessTexture(
    format: TextureFormat.Rgba8888,
    filterMode: TextureFilterMode.Point,
    wrapMode: TextureWrapMode.Clamp
  );
}
