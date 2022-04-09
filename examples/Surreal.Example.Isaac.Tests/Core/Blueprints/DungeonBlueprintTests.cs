namespace Isaac.Core.Blueprints;

public class DungeonBlueprintTests
{
  [Test]
  public void it_should_plan_a_valid_dungeon()
  {
    var blueprint = new DungeonBlueprint
    {
      new PlaceSpawn(),
      new PlaceShop(),
    };

    blueprint.Create().Should().NotBeNull();
  }
}
