using System.Xml.Linq;
using Surreal.IO.Xml;
using Surreal.Mathematics;
using Surreal.Objects;

namespace Isaac.Dungeons;

/// <summary>A blueprint for a dungeon.</summary>
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
}

[Template(typeof(DungeonBlueprint))]
public sealed record DungeonBlueprintTemplate : ITemplate<DungeonBlueprint>
{
  public int Width  { get; init; } = 15;
  public int Height { get; init; } = 9;

  public DungeonBlueprint Create()
  {
    return new DungeonBlueprint
    {
      Width  = Width,
      Height = Height,
    };
  }
}

[XmlSerializer(typeof(DungeonBlueprintTemplate))]
public sealed class DungeonBlueprintTemplateSerializer : XmlSerializer<DungeonBlueprintTemplate>
{
  public override ValueTask<DungeonBlueprintTemplate> DeserializeAsync(XElement element, IXmlSerializationContext context, CancellationToken cancellationToken = default)
  {
    var blueprint = new DungeonBlueprintTemplate
    {
      Width  = (int) element.Attribute("Width")!,
      Height = (int) element.Attribute("Height")!,
    };

    return ValueTask.FromResult(blueprint);
  }
}
