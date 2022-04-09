using Surreal.Graphs;
using Surreal.Objects;

namespace Isaac.Core.Dungeons;

/// <summary>A plan for a dungeon.</summary>
public sealed record DungeonPlan(Random Random, int Width, int Height);

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
}

/// <summary>A blueprint for a <see cref="DungeonPlan"/>.</summary>
public sealed record DungeonBlueprint : DungeonNode, ITemplate<DungeonPlan>
{
  public Seed   Seed        { get; init; }
  public int    Width       { get; init; } = 15;
  public int    Height      { get; init; } = 9;
  public string Description { get; init; } = string.Empty;

  public DungeonPlan Create()
  {
    return Plan(new DungeonPlan(Seed.ToRandom(), Width, Height));
  }
}

/// <summary>Places a shop in the dungeon.</summary>
public sealed record PlaceShop : DungeonNode
{
  public void ApplyTo(DungeonPlan plan)
  {
  }
}

/// <summary>Places a spawn point in the dungeon.</summary>
public sealed record PlaceSpawn : DungeonNode
{
  public void ApplyTo(DungeonPlan plan)
  {
  }
}
