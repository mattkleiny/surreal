using Surreal.Mathematics;

namespace Isaac.Dungeons;

/// <summary>A contribution to a <see cref="DungeonPlan"/>.</summary>
public interface IDungeonPlanContribution
{
  void ApplyTo(DungeonPlan plan);
}

/// <summary>A plan for a <see cref="Dungeon"/>.</summary>
public sealed record DungeonPlan(Seed Seed, int Width, int Height)
{
  public Dungeon Spawn(IActorContext context)
  {
    return new Dungeon(context);
  }
}
