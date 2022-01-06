using Isaac.Dungeons;
using Isaac.Dungeons.Nodes;

namespace Surreal.Dungeons;

public class DungeonBlueprintTests
{
  [Test]
  public void it_should_plan_a_valid_dungeon_plan()
  {
    var blueprint = new DungeonBlueprint
    {
      new PlaceSpawn(),
      new PlaceShop(),
    };

    Assert.IsNotNull(blueprint.CreatePlan());
  }
}
