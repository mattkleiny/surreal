﻿using Surreal.Assets;
using Surreal.Graphics.Utilities;

namespace Surreal.Colors;

public class ColorPaletteLoaderTests
{
  [Test]
  public async Task it_should_load_JASC_palettes()
  {
    using var manager = new AssetManager();

    manager.AddLoader(new ColorPaletteLoader());

    var palette = await manager.LoadAsync<ColorPalette>("Assets/External/palettes/low-8.pal");

    palette.Value.Count.Should().Be(8);
  }
}
