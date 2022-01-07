using System.Xml.Linq;
using Surreal.Graphs;
using Surreal.IO.Xml;
using Surreal.Mathematics;
using Surreal.Objects;

namespace Isaac.Blueprints;

/// <summary>A plan for a dungeon.</summary>
public sealed record DungeonPlan(Seed Seed, int Width, int Height);

/// <summary>Base class for any node in a dungeon plan.</summary>
public abstract record DungeonNode : GraphNode<DungeonNode>
{
  protected virtual DungeonPlan Plan(DungeonPlan plan)
  {
    foreach (var child in Children)
    {
      child.Plan(plan);
    }

    return plan;
  }

  /// <summary>A blueprint for a <see cref="DungeonPlan"/>.</summary>
  [Template(typeof(DungeonPlan))]
  public sealed record DungeonBlueprint : DungeonNode, ITemplate<DungeonPlan>
  {
    public Seed   Seed        { get; init; }
    public int    Width       { get; init; } = 15;
    public int    Height      { get; init; } = 9;
    public string Description { get; init; } = string.Empty;

    public DungeonPlan Create()
    {
      return Plan(new DungeonPlan(Seed, Width, Height));
    }

    /// <summary>The <see cref="XmlSerializer{T}"/> for this node.</summary>
    [XmlSerializer(typeof(DungeonBlueprint))]
    private sealed class Serializer : XmlSerializer<DungeonBlueprint>
    {
      public override ValueTask<DungeonBlueprint> DeserializeAsync(XElement element, IXmlSerializationContext context, CancellationToken cancellationToken = default)
      {
        return ValueTask.FromResult(new DungeonBlueprint
        {
          Seed        = Seed.Randomized,
          Width       = (int) element.Attribute("Width")!,
          Height      = (int) element.Attribute("Height")!,
          Description = (string) element.Attribute("Description")!,
        });
      }
    }
  }

  /// <summary>Places a spawn point in the dungeon.</summary>
  public sealed record PlaceSpawn : DungeonNode
  {
    public void ApplyTo(DungeonPlan plan)
    {
    }
  }

  /// <summary>Places a shop in the dungeon.</summary>
  public sealed record PlaceShop : DungeonNode
  {
    public void ApplyTo(DungeonPlan plan)
    {
    }
  }
}
