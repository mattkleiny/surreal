using Surreal.Colors;
using Surreal.Entities;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Sprites;

/// <summary>
/// A sprite component.
/// </summary>
public record struct Sprite() : IComponent<Sprite>
{
  /// <summary>
  /// The region of the texture to render.
  /// </summary>
  public TextureRegion Region { get; set; }

  /// <summary>
  /// The tint color.
  /// </summary>
  public Color32 Tint { get; set; } = Color32.White;
}
