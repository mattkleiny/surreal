using Surreal.Graphics.Materials;
using Surreal.Graphics.Textures;

namespace Isaac.Actors;

/// <summary>A transformed position in 2-space.</summary>
public record struct Transform
{
  private float rotation;

  public Vector2 Position { get; set; }
  public Vector2 Scale    { get; set; }

  public float Rotation
  {
    get => rotation;
    set => rotation = value.Clamp(0, 2 * MathF.PI);
  }
}

/// <summary>A sprite to be rendered in 2-space.</summary>
public record struct Sprite
{
  public TextureRegion Texture  { get; set; }
  public Material?     Material { get; set; }
}

/// <summary>Character statistics.</summary>
public record struct Statistics
{
  private int health;
  private int bombs;
  private int coins;

  public int Health
  {
    get => health;
    set => health = value.Clamp(0, 100);
  }

  public int Bombs
  {
    get => bombs;
    set => bombs = value.Clamp(0, 99);
  }

  public int Coins
  {
    get => coins;
    set => coins = value.Clamp(0, 99);
  }
}
