using System.Xml.Linq;
using Surreal.Graphics.Images;
using Surreal.Graphs;
using Surreal.IO.Xml;
using Surreal.Mathematics;
using Surreal.Objects;

namespace Isaac.Sprites;

public abstract record SpriteBlueprintNode : GraphNode<SpriteBlueprintNode>
{
  protected virtual void Plan(SpritePlan plan)
  {
    foreach (var child in Children)
    {
      child.Plan(plan);
    }
  }

  protected sealed record SpritePlan(int Width, int Height)
  {
    public Color32[,] Colors { get; } = new Color32[Width, Height];

    public Image ToImage()
    {
      var image  = new Image(Width, Height);
      var pixels = image.Pixels;

      for (var y = 0; y < Height; y++)
      for (var x = 0; x < Width; x++)
      {
        pixels[x + y * Width] = Colors[x, y];
      }

      return image;
    }
  }
}

[Template(typeof(Image))]
public sealed record SpriteBlueprint : SpriteBlueprintNode, ITemplate<Image>
{
  public Seed Seed   { get; init; }
  public int  Width  { get; init; } = 16;
  public int  Height { get; init; } = 16;

  public Image Create()
  {
    var result = new SpritePlan(Width, Height);

    Plan(result);

    return result.ToImage();
  }
}

[XmlSerializer(typeof(SpriteBlueprint))]
public sealed class SpriteBlueprintSerializer : XmlSerializer<SpriteBlueprint>
{
  public override ValueTask<SpriteBlueprint> DeserializeAsync(XElement element, IXmlSerializationContext context, CancellationToken cancellationToken = default)
  {
    return ValueTask.FromResult(new SpriteBlueprint
    {
      Seed = Seed.Randomized,
    });
  }
}
