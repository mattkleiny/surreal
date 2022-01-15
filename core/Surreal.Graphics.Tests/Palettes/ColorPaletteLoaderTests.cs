﻿using Surreal.Assets;

namespace Surreal.Graphics.Palettes;

public class ColorPaletteLoaderTests
{
  [Test]
  public async Task it_should_load_JASC_palettes()
  {
    using var manager = new AssetManager();

    manager.AddLoader(new ColorPaletteLoader());

    var palette = await manager.LoadAssetAsync<ColorPalette>("Assets/palettes/low-8.pal");

    Assert.AreEqual(8, palette.Count);
  }
}