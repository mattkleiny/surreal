using Isaac.Dungeons.Nodes;
using Surreal.Mathematics;
using Surreal.Objects.Templates;

namespace Isaac.Dungeons;

public sealed class DungeonBlueprint : BlueprintNode
{
  public int Width  { get; init; } = 15;
  public int Height { get; init; } = 9;

  public DungeonPlan CreatePlan(Seed seed)
  {
    var plan = new DungeonPlan(seed, Width, Height);

    PlanDungeon(plan);

    return plan;
  }

  public sealed class Template : IImportableTemplate<DungeonBlueprint>
  {
    public int Width  { get; set; }
    public int Height { get; set; }

    public DungeonBlueprint Create()
    {
      return new DungeonBlueprint
      {
        Width  = Width,
        Height = Height,
      };
    }

    public void OnImportTemplate(ITemplateImportContext context)
    {
      Width  = context.Parse(nameof(Width), 15);
      Height = context.Parse(nameof(Height), 15);
    }
  }
}
