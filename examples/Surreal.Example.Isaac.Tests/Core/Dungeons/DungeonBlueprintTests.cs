namespace Isaac.Core.Dungeons;

public class DungeonBlueprintTests
{
  [Test]
  public void it_should_plan_a_valid_dungeon()
  {
    var blueprint = DungeonBlueprint.Simple;

    blueprint.Create(Seed.Default).Should().NotBeNull();
  }
}
