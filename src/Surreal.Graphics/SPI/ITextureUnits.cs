using Surreal.Graphics.Textures;

namespace Surreal.Graphics.SPI
{
  public interface ITextureUnits
  {
    Texture? this[int unit] { get; set; }
  }
}
