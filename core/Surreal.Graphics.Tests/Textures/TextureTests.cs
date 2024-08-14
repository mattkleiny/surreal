using Surreal.Graphics.Meshes;

namespace Surreal.Graphics.Textures;

public class TextureTests
{
  [Test]
  public void it_should_create_a_mesh()
  {
    var device = Substitute.For<IGraphicsDevice>();

    using var mesh = new Mesh<Vertex2>(device);

    device.Received(1).CreateMesh(Arg.Any<GraphicsHandle>(), Arg.Any<GraphicsHandle>(), mesh.Descriptors);
  }

  [Test]
  public void it_should_dispose_mesh()
  {
    var device = Substitute.For<IGraphicsDevice>();
    var mesh = new Mesh<Vertex2>(device);

    mesh.Dispose();

    device.Received(1).DeleteMesh(mesh.Handle);
  }
}
