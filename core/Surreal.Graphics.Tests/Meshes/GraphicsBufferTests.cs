namespace Surreal.Graphics.Meshes;

public class GraphicsBufferTests
{
  [Test]
  public void it_should_create_a_buffer()
  {
    var device = Substitute.For<IGraphicsDevice>();

    using var buffer = new GraphicsBuffer<int>(device, BufferType.Vertex);

    device.Received(1).CreateBuffer(BufferType.Vertex, BufferUsage.Static);
  }

  [Test]
  public void it_should_dispose_buffer()
  {
    var device = Substitute.For<IGraphicsDevice>();
    var buffer = new GraphicsBuffer<int>(device, BufferType.Vertex);

    buffer.Dispose();

    device.Received(1).DeleteBuffer(buffer.Handle);
  }
}
