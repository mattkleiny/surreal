using Surreal.Graphics.Images;
using Surreal.Graphs;
using Surreal.IO;
using Surreal.Memory;
using Surreal.Objects;

namespace Isaac.Core.Sprites;

/// <summary>An in-memory plan for a sprite.</summary>
public sealed class SpritePlan
{
  private readonly Color32[] pixels;

  public SpritePlan(Random random, int width, int height)
  {
    Random = random;
    Width = width;
    Height = height;
    pixels = new Color32[width * height];
  }

  public Random            Random { get; }
  public int               Width  { get; }
  public int               Height { get; }
  public SpanGrid<Color32> Pixels => new(pixels, Width);

  public Image ToImage()
  {
    var image = new Image(Width, Height);
    var pixels = image.Pixels;

    for (var y = 0; y < Height; y++)
    for (var x = 0; x < Width; x++)
    {
      pixels[x, y] = this.pixels[x + y * Width];
    }

    return image;
  }
}

/// <summary>A graph node for procedurally generating sprites.</summary>
public abstract record SpriteNode : GraphNode<SpriteNode>
{
  protected virtual SpritePlan Plan(SpritePlan plan)
  {
    foreach (var child in Children)
    {
      plan = child.Plan(plan);
    }

    return plan;
  }
}

/// <summary>A blueprint for a <see cref="SpritePlan"/>.</summary>
public sealed record SpriteBlueprint : SpriteNode, ITemplate<SpritePlan>
{
  public Seed Seed   { get; init; } = Seed.Default;
  public int  Width  { get; init; } = 15;
  public int  Height { get; init; } = 9;

  public SpritePlan Create()
  {
    return Plan(new SpritePlan(Seed.ToRandom(), Width, Height));
  }
}

/// <summary>Fills the image with random colors</summary>
public sealed record FillWithColor(Color32 Color) : SpriteNode
{
  protected override SpritePlan Plan(SpritePlan plan)
  {
    var pixels = plan.Pixels;

    for (var y = 0; y < plan.Height; y++)
    for (var x = 0; x < plan.Width; x++)
    {
      pixels[x, y] = Color;
    }

    return plan;
  }
}

/// <summary>Fills the image with random colors</summary>
public sealed record FillRandomly(float Chance, Color32 Color) : SpriteNode
{
  protected override SpritePlan Plan(SpritePlan plan)
  {
    var pixels = plan.Pixels;

    for (var y = 0; y < plan.Height; y++)
    for (var x = 0; x < plan.Width; x++)
    {
      if (plan.Random.NextChance(Chance))
      {
        pixels[x, y] = Color;
      }
    }

    return plan;
  }
}

/// <summary>Samples from the given image randomly.</summary>
public sealed record SampleImage(VirtualPath Path) : SpriteNode;
