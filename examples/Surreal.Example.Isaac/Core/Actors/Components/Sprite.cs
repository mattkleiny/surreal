using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;

namespace Isaac.Core.Actors.Components;

/// <summary>A sprite to be rendered in 2-space.</summary>
public record struct Sprite()
{
  /// <summary>The texture to apply to the sprite.</summary>
  public TextureRegion Texture = default;

  /// <summary>The material to use when rendering the sprite.</summary>
  public Material? Material = null;

  /// <summary>The tint color of the sprite.</summary>
  public Color Tint = Color.White;
}
