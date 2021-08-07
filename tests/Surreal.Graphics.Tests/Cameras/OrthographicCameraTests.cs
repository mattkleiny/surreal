using System.Numerics;
using Xunit;

namespace Surreal.Graphics.Cameras
{
  public class OrthographicCameraTests
  {
    private readonly OrthographicCamera camera = new(viewportWidth: 640, viewportHeight: 480);

    [Fact]
    public void it_should_project_coordinates_correctly()
    {
      var (x, y) = camera.Project(Vector3.Zero);

      Assert.Equal(320, x);
      Assert.Equal(240, y);
    }
  }
}