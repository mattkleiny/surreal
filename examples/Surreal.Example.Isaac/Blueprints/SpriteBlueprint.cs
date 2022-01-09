using System.Xml.Linq;
using Surreal.Graphics.Images;
using Surreal.Graphs;
using Surreal.IO;
using Surreal.IO.Xml;
using Surreal.Objects;

namespace Isaac.Blueprints;

/// <summary>A graph node for procedurally generating sprites.</summary>
public abstract record SpriteBlueprintNode : GraphNode<SpriteBlueprintNode>
{
  protected virtual SpritePlan Plan(SpritePlan plan)
  {
    foreach (var child in Children)
    {
      plan = child.Plan(plan);
    }

    return plan;
  }

  /// <summary>An in-memory plan for a sprite.</summary>
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

  /// <summary>A blueprint for a sprite <see cref="Image"/>.</summary>
  [Template(typeof(Image))]
  public sealed record SpriteBlueprint : SpriteBlueprintNode, ITemplate<Image>
  {
    public Seed   Seed        { get; init; }
    public int    Width       { get; init; } = 16;
    public int    Height      { get; init; } = 16;
    public string Description { get; init; } = string.Empty;

    public Image Create() => Plan(new SpritePlan(Width, Height)).ToImage();

    /// <summary>The <see cref="XmlSerializer{T}"/> for this node.</summary>
    [XmlSerializer(typeof(SpriteBlueprint), Name = "Sprite")]
    private sealed class Serializer : XmlSerializer<SpriteBlueprint>
    {
      public override async ValueTask<SpriteBlueprint> DeserializeAsync(XElement element, IXmlSerializationContext context, CancellationToken cancellationToken = default)
      {
        return new SpriteBlueprint
        {
          Seed        = Seed.Randomized,
          Width       = (int) element.Attribute("Width")!,
          Height      = (int) element.Attribute("Height")!,
          Description = (string) element.Attribute("Description")!,
          Children = await element.Elements()
            .SelectAsync(async _ => (SpriteBlueprintNode) await context.DeserializeAsync(_, cancellationToken))
            .ToListAsync(cancellationToken),
        };
      }
    }
  }

  /// <summary>Samples sub-images randomly for each pixel in the plan.</summary>
  public sealed record SampleRandomlyNode : SpriteBlueprintNode
  {
    /// <summary>The <see cref="XmlSerializer{T}"/> for this node.</summary>
    [XmlSerializer(typeof(SampleRandomlyNode), Name = "SampleRandomly")]
    private sealed class Serializer : XmlSerializer<SampleRandomlyNode>
    {
      public override async ValueTask<SampleRandomlyNode> DeserializeAsync(XElement element, IXmlSerializationContext context, CancellationToken cancellationToken = default)
      {
        return new SampleRandomlyNode
        {
          Children = await DeserializeChildrenAsync(element, context, cancellationToken),
        };
      }
    }
  }

  /// <summary>Samples from the given images randomly.</summary>
  public sealed record SampleImageNode : SpriteBlueprintNode
  {
    public VirtualPath Path { get; init; }

    /// <summary>The <see cref="XmlSerializer{T}"/> for this node.</summary>
    [XmlSerializer(typeof(SampleImageNode), Name = "SampleImage")]
    private sealed class Serializer : XmlSerializer<SampleImageNode>
    {
      public override ValueTask<SampleImageNode> DeserializeAsync(XElement element, IXmlSerializationContext context, CancellationToken cancellationToken = default)
      {
        return ValueTask.FromResult(new SampleImageNode
        {
          Path = (string) element.Attribute("Path")!,
        });
      }
    }
  }

  /// <summary>Deserializes all child <see cref="SpriteBlueprintNode"/>s from the given element.</summary>
  private static ValueTask<List<SpriteBlueprintNode>> DeserializeChildrenAsync(XElement element, IXmlSerializationContext context, CancellationToken cancellationToken)
  {
    return element
      .Elements()
      .SelectAsync(async child => (SpriteBlueprintNode) await context.DeserializeAsync(child, cancellationToken))
      .ToListAsync(cancellationToken);
  }
}
