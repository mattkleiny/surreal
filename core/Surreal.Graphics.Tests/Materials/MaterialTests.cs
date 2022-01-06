namespace Surreal.Graphics.Materials;

public class MaterialTests
{
  private static MaterialProperty<float>   Intensity { get; } = new("_Amount");
  private static MaterialProperty<Vector2> Direction { get; } = new("_Direction");

  [Test, AutoFixture]
  public void it_should_set_properties_on_underlying_shader(Material material)
  {
    material.SetProperty(Intensity, 1.0f);
    material.SetProperty(Direction, new Vector2(1f, 1f));

    material.Program.Received(1).SetUniform("_Amount", 1.0f);
    material.Program.Received(1).SetUniform("_Direction", new Vector2(1, 1f));
  }
}
