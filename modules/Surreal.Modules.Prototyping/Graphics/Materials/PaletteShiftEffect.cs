using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Materials;

/// <summary>An <see cref="Effect" /> for the palette shift shader.</summary>
public sealed class PaletteShiftEffect : Effect
{
  private readonly Texture _texture;

  public PaletteShiftEffect(Material material)
    : base(material)
  {
    _texture = new Texture(material.Server);

    Locals.SetProperty(PaletteTexture, _texture, 1);
  }

  private static MaterialProperty<Texture> PaletteTexture { get; } = new("u_palette");
  private static MaterialProperty<int> PaletteWidth { get; } = new("u_paletteWidth");

  public ColorPalette Palette
  {
    set
    {
      _texture.WriteColorPalette(value);

      Locals.SetProperty(PaletteWidth, value.Count);
    }
  }

  public override void Dispose()
  {
    _texture.Dispose();

    base.Dispose();
  }
}


