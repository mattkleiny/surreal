using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Images.Sprites;

/// <summary>A single sprite image and it's metadata.</summary>
public sealed record Sprite(TextureRegion Region, Vector2 Pivot)
{
  public static Sprite Create(Texture texture) => new(texture, new Vector2(0.5f, 0.5f));
  public static Sprite Create(TextureRegion region) => new(region, new Vector2(0.5f, 0.5f));
}
