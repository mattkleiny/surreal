using NUnit.Framework;

namespace Surreal.Graphics.Cameras;

public sealed class PerspectiveCameraTests
{
  private readonly PerspectiveCamera camera = new(viewportWidth: 640, viewportHeight: 480);

  [Test]
  public void it_should_project_coordinates_correctly()
  {
    var (x, y) = camera.Project(Vector3.Zero);

    Assert.AreEqual(320, x);
    Assert.AreEqual(240, y);
  }
}
