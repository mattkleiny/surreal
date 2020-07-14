using Surreal.Graphics.Textures;

namespace Surreal.Graphics {
  public interface ITextureUnits {
    Texture? this[int unit] { get; set; }
  }
}