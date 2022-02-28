using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;

namespace Isaac.Core.Actors.Components;

/// <summary>A sprite to be rendered in 2-space.</summary>
public record struct Sprite()
{
  public TextureRegion Texture  { get; set; } = default;
  public Material?     Material { get; set; } = null;
  public Color         Tint     { get; set; } = Color.White;
}
