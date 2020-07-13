using Surreal.Graphics.SPI;
using Surreal.Graphics.Textures;

namespace Surreal.Platform.Internal.Graphics {
  internal sealed class HeadlessTextureUnits : ITextureUnits {
    public Texture? this[int unit] {
      get => null;
      set {
        // no-op
      }
    }
  }
}