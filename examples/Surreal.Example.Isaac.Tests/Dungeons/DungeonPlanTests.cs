using Isaac.Core.Blueprints;

namespace Isaac.Dungeons;

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

    blueprint.Create().Should().NotBeNull();
  }
}
