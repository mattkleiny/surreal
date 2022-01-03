using Isaac.Dungeons;
using Isaac.Dungeons.Nodes;
using NUnit.Framework;

namespace Surreal.Dungeons;

public class DungeonBlueprintTests
{
  [Test]
  public void it_should_plan_a_non_null_dungeon_plan()
  {
    var blueprint = new DungeonBlueprint
    {
      new PlaceSpawn(),
      new PlaceShop(),
    };

    Assert.IsNotNull(blueprint.CreatePlan());
  }
}
