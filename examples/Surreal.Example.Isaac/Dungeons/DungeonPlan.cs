using Surreal.Mathematics;

namespace Isaac.Dungeons;

public sealed class DungeonPlan
{
  public DungeonPlan(Seed seed, int width, int height)
  {
    Seed   = seed;
    Width  = width;
    Height = height;
  }

  public Seed Seed   { get; }
  public int  Width  { get; }
  public int  Height { get; }

  public Dungeon CreateDungeon()
  {
    throw new NotImplementedException();
  }
}
