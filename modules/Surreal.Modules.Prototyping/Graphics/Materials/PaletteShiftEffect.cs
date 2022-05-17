using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Materials;

/// <summary>A <see cref="Effect"/> for the palette shift material.</summary>
public sealed class PaletteShiftEffect : Effect
{
  private static MaterialProperty<Texture> PaletteTexture { get; } = new("u_palette");
  private static MaterialProperty<int>     PaletteWidth   { get; } = new("u_paletteWidth");

  private readonly Texture texture;

  public PaletteShiftEffect(Material material)
    : base(material)
  {
    texture = new Texture(material.Server);

    Locals.SetProperty(PaletteTexture, texture, slot: 1);
  }

  public ColorPalette Palette
  {
    set
    {
      texture.WriteColorPalette(value);

      Locals.SetProperty(PaletteWidth, value.Count);
    }
  }

  public override void Dispose()
  {
    texture.Dispose();

    base.Dispose();
  }
}
