using Surreal.Mathematics;
using Surreal.Objects;

namespace Isaac.Dungeons;

/// <summary>A blueprint for a dungeon.</summary>
[Template(typeof(DungeonBlueprintTemplate))]
public sealed record DungeonBlueprint : DungeonNode
{
  public int Width  { get; init; } = 15;
  public int Height { get; init; } = 9;

  public DungeonPlan CreatePlan(Seed seed = default)
  {
    var plan = new DungeonPlan(seed, Width, Height);

    foreach (var node in GetChildrenRecursively())
    {
      if (node is IDungeonPlanContribution contribution)
      {
        contribution.ApplyTo(plan);
      }
    }

    return plan;
  }

  private sealed record DungeonBlueprintTemplate : IImportableTemplate<DungeonBlueprint>
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
      Height = context.Parse(nameof(Height), 9);
    }
  }
}
