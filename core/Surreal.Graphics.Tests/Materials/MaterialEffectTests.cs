namespace Surreal.Graphics.Materials;

public class MaterialEffectTests
{
  private static MaterialProperty<float>   Intensity { get; } = new("_Amount");
  private static MaterialProperty<Vector2> Direction { get; } = new("_Direction");

  [Test, AutoFixture]
  public void it_should_set_effect_properties_on_underlying_shader(Material material)
  {
    var effect = new MaterialEffect
    {
      { Intensity, 1.0f },
      { Direction, new Vector2(1f, 1f) },
    };

    material.ApplyEffect(effect);

    material.Program.Received(1).SetUniform("_Amount", 1.0f);
    material.Program.Received(1).SetUniform("_Direction", new Vector2(1, 1f));
  }
}
