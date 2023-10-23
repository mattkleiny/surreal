using Surreal.Graphics.Meshes;

namespace Surreal.Graphics.Tests.Meshes;

public class GraphicsBufferTests
{
  [Test]
  public void it_should_create_a_buffer()
  {
    var backend = Substitute.For<IGraphicsBackend>();

    using var buffer = new GraphicsBuffer<int>(backend, BufferType.Vertex);

    backend.Received(1).CreateBuffer();
  }

  [Test]
  public void it_should_dispose_buffer()
  {
    var backend = Substitute.For<IGraphicsBackend>();
    var buffer = new GraphicsBuffer<int>(backend, BufferType.Vertex);

    buffer.Dispose();

    backend.Received(1).DeleteBuffer(buffer.Handle);
  }
}
