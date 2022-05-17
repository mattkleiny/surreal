namespace Surreal.Graphics.Materials;

/// <summary>A <see cref="Effect"/> for the palette shift material.</summary>
public sealed class AberrationEffect : Effect
{
  public AberrationEffect(Material material)
    : base(material)
  {
  }

  public float Intensity
  {
    get => Locals.GetProperty(MaterialProperty.Intensity);
    set => Locals.SetProperty(MaterialProperty.Intensity, value);
  }
}
