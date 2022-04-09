using Surreal.Graphics.Textures;

namespace Avventura.Core.Actors.Components;

/// <summary>A sprite to be rendered in 2-space.</summary>
public record struct Sprite
{
  /// <summary>The texture to apply to the sprite.</summary>
  public TextureRegion Texture = default;

  /// <summary>The tint color of the sprite.</summary>
  public Color Tint = Color.White;
}
