using Surreal.Colors;

namespace Surreal.Graphics.Materials;

public class MaterialPropertyTests
{
  private static MaterialProperty<Matrix4x4> ProjectionView { get; } = new("u_projectionView");
  private static MaterialProperty<Color32> ClearColor { get; } = new("u_clearColor");
  private static MaterialProperty<Vector3> LightPosition { get; } = new("u_lightPosition");

  [Test]
  public void it_should_read_and_write_matrix()
  {
    var propertySet = new MaterialPropertySet();

    propertySet.SetProperty(ProjectionView, Matrix4x4.Identity);
  }

  [Test]
  public void it_should_read_and_write_color()
  {
    var propertySet = new MaterialPropertySet();

    propertySet.SetProperty(ClearColor, Color32.White);
  }

  [Test]
  public void it_should_read_and_write_vector()
  {
    var propertySet = new MaterialPropertySet();

    propertySet.SetProperty(LightPosition, Vector3.One);
  }
}
