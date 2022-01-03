using Surreal.Graphics.Images;
using Surreal.Graphics.Textures;

namespace Surreal.Internal.Graphics.Resources;

internal sealed class HeadlessTexture : Texture
{
  public HeadlessTexture(TextureFormat format, TextureFilterMode filterMode, TextureWrapMode wrapMode)
    : base(format, filterMode, wrapMode)
  {
  }

  protected override void Upload(ITextureData? existingData, ITextureData newData)
  {
    // no-op
  }

  public override Image Download()
  {
    return new Image(0, 0);
  }
}
