using Isaac.Blueprints;
using Isaac.Dungeons;

namespace Surreal.Dungeons;

public class DungeonPlanTests
{
  [Test]
  public void it_should_plan_a_valid_dungeon_plan()
  {
    var blueprint = new DungeonNode.DungeonBlueprint
    {
      new DungeonNode.PlaceSpawn(),
      new DungeonNode.PlaceShop(),
    };

    Assert.IsNotNull(blueprint.Create());
  }
}
