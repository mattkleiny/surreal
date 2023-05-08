using Surreal.Colors;
using Surreal.Resources;

namespace Surreal.Graphics;

public class ColorPaletteLoaderTests
{
  [Test]
  public async Task it_should_load_JASC_palettes()
  {
    using var manager = new ResourceManager();

    manager.AddLoader(new ColorPaletteLoader());

    var palette = await manager.LoadResourceAsync<ColorPalette>("Assets/palettes/low-8.pal");

    palette.Count.Should().Be(8);
  }
}
