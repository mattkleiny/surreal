using Surreal.Assets;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics;

/// <summary>Common graphics utilities and extensions.</summary>
public static class GraphicsExtensions
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

  public static async Task<Material> LoadAberrationMaterialAsync(this IAssetManager assets)
  {
    return await assets.LoadAssetAsync<Material>("resx://Surreal/Resources/shaders/aberration.glsl");
  }

  /// <summary>Writes the colors from the given <see cref="ColorPalette"/> into the texture.</summary>
  public static void WriteColorPalette(this Texture texture, ColorPalette palette)
  {
    texture.WritePixels(palette.Count, 1, palette.Span);
  }
}
