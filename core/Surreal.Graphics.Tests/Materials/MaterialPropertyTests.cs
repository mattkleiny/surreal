using Surreal.Colors;
using Surreal.Graphics.Materials;

namespace Surreal.Graphics.Tests.Materials;

public class MaterialPropertyTests
{
  private static MaterialProperty<Matrix4x4> Transform { get; } = new("u_transform");
  private static MaterialProperty<Color32> ClearColor { get; } = new("u_clearColor");
  private static MaterialProperty<Vector3> LightPosition { get; } = new("u_lightPosition");

  [Test]
  public void it_should_read_and_write_matrix()
  {
    var propertySet = new MaterialPropertySet();

    propertySet.SetProperty(Transform, Matrix4x4.Identity);
    propertySet.GetPropertyOrThrow(Transform).Should().Be(Matrix4x4.Identity);
  }

  [Test]
  public void it_should_read_and_write_color()
  {
    var propertySet = new MaterialPropertySet();

    propertySet.SetProperty(ClearColor, Color32.White);
    propertySet.GetPropertyOrThrow(ClearColor).Should().Be(Color32.White);
  }

  [Test]
  public void it_should_read_and_write_vector()
  {
    var propertySet = new MaterialPropertySet();

    propertySet.SetProperty(LightPosition, Vector3.One);
    propertySet.GetPropertyOrThrow(LightPosition).Should().Be(Vector3.One);
  }
}
