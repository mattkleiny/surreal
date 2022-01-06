using Isaac.Dungeons;

namespace Surreal.Dungeons;

public class DungeonBlueprintTests
{
  [Test]
  public void it_should_plan_a_valid_dungeon_plan()
  {
    var blueprint = new DungeonBlueprint
    {
      new DungeonNode.PlaceSpawn(),
      new DungeonNode.PlaceShop(),
    };

    Assert.IsNotNull(blueprint.CreatePlan());
  }
}
