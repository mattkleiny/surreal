using Surreal.Graphics.Images;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Utilities;

/// <summary>An in-memory <see cref="Texture"/> for testing purposes.</summary>
internal sealed class InMemoryTexture : Texture
{
  public InMemoryTexture(TextureFormat format, TextureFilterMode filterMode, TextureWrapMode wrapMode)
    : base(format, filterMode, wrapMode)
  {
  }

  public ITextureData? TextureData { get; private set; }

  protected override void Upload(ITextureData? existingData, ITextureData newData)
  {
    TextureData = newData;
  }

  public override Image Download()
  {
    if (TextureData is not Image image)
    {
      throw new NotSupportedException("A non-image texture data was provided");
    }

    return image;
  }
}
