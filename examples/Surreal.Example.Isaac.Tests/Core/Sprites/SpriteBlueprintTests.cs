namespace Isaac.Core.Sprites;

public class SpriteBlueprintTests
{
  [Test]
  public void it_should_plan_a_valid_sprite()
  {
    var blueprint = new SpriteBlueprint
    {
      new FillRandomly(0.5f, Color.White),
    };

    var plan = blueprint.Create();
    var image = plan.ToImage();

    image.Should().NotBeNull();
  }
}
