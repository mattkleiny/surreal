using Surreal.Graphics.Meshes;

namespace Surreal.Graphics.Textures;

public class TextureTests
{
  [Test]
  public void it_should_create_a_mesh()
  {
    var backend = Substitute.For<IGraphicsBackend>();

    using var mesh = new Mesh<Vertex2>(backend);

    backend.Received(1).CreateMesh(Arg.Any<GraphicsHandle>(), Arg.Any<GraphicsHandle>(), mesh.Descriptors);
  }

  [Test]
  public void it_should_dispose_mesh()
  {
    var backend = Substitute.For<IGraphicsBackend>();
    var mesh = new Mesh<Vertex2>(backend);

    mesh.Dispose();

    backend.Received(1).DeleteMesh(mesh.Handle);
  }
}
