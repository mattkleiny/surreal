namespace Avventura.Core.Sprites;

public class SpriteBlueprintTests
{
  [Test]
  public void it_should_plan_a_valid_sprite()
  {
    var blueprint = new SpriteBlueprint
    {
      new FillImage(Color.White, 0.5f),
    };

    var plan = blueprint.Create();
    var image = plan.ToImage();

    image.Should().NotBeNull();
  }
}
