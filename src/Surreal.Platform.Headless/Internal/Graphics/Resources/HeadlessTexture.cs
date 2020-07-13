using Surreal.Graphics.Textures;

namespace Surreal.Platform.Internal.Graphics.Resources {
  internal sealed class HeadlessTexture : Texture {
    public HeadlessTexture(TextureFormat format, TextureFilterMode filterMode, TextureWrapMode wrapMode)
        : base(format, filterMode, wrapMode) {
    }

    protected override void Upload(ITextureData? existingData, ITextureData newData) {
      // no-op
    }

    public override void Download(Image image) {
      // no-op
    }
  }
}