namespace Surreal.Graphics.Cameras;

public sealed class OrthographicCameraTests
{
  private readonly OrthographicCamera camera = new(viewportWidth: 640, viewportHeight: 480);

  [Test]
  public void it_should_project_coordinates_correctly()
  {
    var (x, y) = camera.Project(Vector3.Zero);

    x.Should().Be(320);
    y.Should().Be(240);
  }
}
