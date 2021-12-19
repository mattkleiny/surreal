using System.Numerics;
using Surreal.Graphics.Textures;

namespace Isaac.Actors;

/// <summary>A transformed position in 2-space.</summary>
public struct Transform
{
  public Vector2 Position { get; set; }
  public Vector2 Scale    { get; set; }
  public float   Rotation { get; set; }
}

/// <summary>A sprite to be rendered in 2-space.</summary>
public struct Sprite
{
  public TextureRegion Texture { get; set; }
}

/// <summary>Character statistics.</summary>
public struct Statistics
{
  public int Health { get; set; }
  public int Bombs  { get; set; }
  public int Coins  { get; set; }
}
