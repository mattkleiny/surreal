namespace Isaac.Core.Dungeons;

/// <summary>A <see cref="Actor"/> which implements the physics and rendering for a dungeon.</summary>
public sealed class Dungeon : Actor
{
  public Dungeon(DungeonBlueprint blueprint, Seed seed = default)
    : this(blueprint.Create(seed))
  {
  }

  public Dungeon(DungeonPlan plan)
  {
  }
}
