using Surreal.Assets;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics;

/// <summary>Utilities for working with <see cref="ColorPalette"/>s.</summary>
public static class ColorPalettes
{
  public static async Task<ColorPalette> LoadKule16Async(this IAssetManager assets)
  {
    return await assets.LoadAssetAsync<ColorPalette>("resx://Surreal/Resources/palettes/kule-16.pal");
  }

  public static async Task<ColorPalette> LoadLow8Async(this IAssetManager assets)
  {
    return await assets.LoadAssetAsync<ColorPalette>("resx://Surreal/Resources/palettes/low-8.pal");
  }

  public static async Task<ColorPalette> LoadSpaceDust9Async(this IAssetManager assets)
  {
    return await assets.LoadAssetAsync<ColorPalette>("resx://Surreal/Resources/palettes/space-dust-9.pal");
  }

  /// <summary>Writes the colors from the given <see cref="ColorPalette"/> into the texture.</summary>
  public static void WriteColorPalette(this Texture texture, ColorPalette palette)
  {
    texture.WritePixels(palette.Count, 1, palette.Span);
  }
}
