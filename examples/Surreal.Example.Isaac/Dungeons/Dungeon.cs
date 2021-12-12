using System.Collections;
using Surreal.Mathematics;
using Surreal.Templates;

namespace Isaac.Dungeons;

public sealed class Dungeon
{
}

public sealed class DungeonPlan
{
  public Seed Seed   { get; }
  public int  Width  { get; }
  public int  Height { get; }

  public DungeonPlan(Seed seed, int width, int height)
  {
    Seed   = seed;
    Width  = width;
    Height = height;
  }

  public Dungeon CreateDungeon()
  {
    return new Dungeon();
  }
}

public abstract class DungeonBlueprintNode : IEnumerable<DungeonBlueprintNode>
{
  private List<DungeonBlueprintNode> Children { get; } = new();

  public void Add(DungeonBlueprintNode node)    => Children.Add(node);
  public void Remove(DungeonBlueprintNode node) => Children.Remove(node);

  protected void PlanDungeon(DungeonPlan plan)
  {
    foreach (var child in Children)
    {
      child.PlanDungeon(plan);
    }
  }

  public IEnumerator<DungeonBlueprintNode> GetEnumerator() => Children.GetEnumerator();
  IEnumerator IEnumerable.                 GetEnumerator() => GetEnumerator();
}

public sealed class DungeonBlueprint : DungeonBlueprintNode
{
  public int Width  { get; init; } = 15;
  public int Height { get; init; } = 9;

  public DungeonPlan CreatePlan(Seed seed)
  {
    var plan = new DungeonPlan(seed, Width, Height);

    PlanDungeon(plan);

    return plan;
  }

  [Template(typeof(DungeonBlueprint))]
  private sealed class Template : IImportableTemplate<DungeonBlueprint>
  {
    public int Width  { get; set; }
    public int Height { get; set; }

    public DungeonBlueprint Create()
    {
      return new DungeonBlueprint
      {
        Width  = Width,
        Height = Height
      };
    }

    public void OnImportTemplate(ITemplateImportContext context)
    {
      Width  = context.Parse(nameof(Width), 15);
      Height = context.Parse(nameof(Height), 15);
    }
  }
}

public sealed class PlaceSpawn : DungeonBlueprintNode
{
}

public sealed class PlaceShop : DungeonBlueprintNode
{
}
