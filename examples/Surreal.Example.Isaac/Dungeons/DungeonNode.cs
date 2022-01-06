using Surreal.Graphs;

namespace Isaac.Dungeons;

/// <summary>Base class for any node in a dungeon plan.</summary>
public abstract record DungeonNode : GraphNode<DungeonNode>
{
  public sealed record PlaceShop : DungeonNode, IDungeonPlanContribution
  {
    public void ApplyTo(DungeonPlan plan)
    {
    }
  }

  public sealed record PlaceSpawn : DungeonNode, IDungeonPlanContribution
  {
    public void ApplyTo(DungeonPlan plan)
    {
    }
  }
}
