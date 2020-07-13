namespace Avventura.Graphics.Palettes {
  public interface IIndexedColorProvider {
    ColorPalette this[ColorPaletteChannel channel] { get; }
  }
}