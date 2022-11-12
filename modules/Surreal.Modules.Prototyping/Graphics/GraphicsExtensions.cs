using Surreal.Assets;
using Surreal.Graphics.Materials;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics;

/// <summary>Some built-in <see cref="ColorPalette" />s.</summary>
public enum BuiltInPalette
{
  Ayy4,
  Demichrome4,
  Hollow4,
  Kule16,
  Low8,
  SpaceDust9
}

/// <summary>Common graphics utilities and extensions.</summary>
public static class GraphicsExtensions
{
  public static async Task<ColorPalette> LoadBuiltInPaletteAsync(this IAssetManager assets, BuiltInPalette palette)
  {
    return await assets.LoadAssetAsync<ColorPalette>(palette switch
    {
      BuiltInPalette.Ayy4 => "resx://Surreal/Resources/palettes/ayy-4.pal",
      BuiltInPalette.Demichrome4 => "resx://Surreal/Resources/palettes/demichrome-4.pal",
      BuiltInPalette.Hollow4 => "resx://Surreal/Resources/palettes/hollow-4.pal",
      BuiltInPalette.Kule16 => "resx://Surreal/Resources/palettes/kule-16.pal",
      BuiltInPalette.Low8 => "resx://Surreal/Resources/palettes/low-8.pal",
      BuiltInPalette.SpaceDust9 => "resx://Surreal/Resources/palettes/space-dust-9.pal",

      _ => throw new ArgumentOutOfRangeException(nameof(palette), palette, null)
    });
  }

  public static async Task<AberrationEffect> LoadAberrationEffectAsync(this IAssetManager assets)
  {
    return new AberrationEffect(await assets.LoadAssetAsync<Material>("resx://Surreal/Resources/shaders/aberration.glsl"));
  }

  public static async Task<PaletteShiftEffect> LoadPaletteShiftEffectAsync(this IAssetManager manager)
  {
    return new PaletteShiftEffect(await manager.LoadAssetAsync<Material>("resx://Surreal/Resources/shaders/palette-shift.glsl"));
  }

  /// <summary>Writes the colors from the given <see cref="ColorPalette" /> into the texture.</summary>
  public static void WriteColorPalette(this Texture texture, ColorPalette palette)
  {
    texture.WritePixels(palette.Count, 1, palette.Span);
  }
}




