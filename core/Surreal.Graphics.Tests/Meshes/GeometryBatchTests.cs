using Surreal.Graphics.Materials;
using Surreal.Graphics.Utilities;
using Surreal.Mathematics;
using static Surreal.Graphics.Meshes.GeometryBatch;

namespace Surreal.Graphics.Meshes;

public class GeometryBatchTests
{
  [Test, AutoFixture]
  public void it_should_generate_a_valid_point_mesh(IGraphicsDevice device, Material material)
  {
    device.CreateBuffer<Vertex>().Returns(new InMemoryGraphicsBuffer<Vertex>());
    device.CreateBuffer<ushort>().Returns(new InMemoryGraphicsBuffer<ushort>());

    using var batch = new GeometryBatch(device);

    batch.Begin(material, Matrix4x4.Identity);
    batch.DrawPoint(Vector2.Zero, Color.White);

    device.Received(1).DrawMesh(Arg.Any<Mesh<Vertex>>(), material, vertexCount: 1, indexCount: 0, MeshType.Points);
  }

  [Test, AutoFixture]
  public void it_should_generate_a_valid_line_mesh(IGraphicsDevice device, Material material)
  {
    device.CreateBuffer<Vertex>().Returns(new InMemoryGraphicsBuffer<Vertex>());
    device.CreateBuffer<ushort>().Returns(new InMemoryGraphicsBuffer<ushort>());

    using var batch = new GeometryBatch(device);

    batch.Begin(material, Matrix4x4.Identity);
    batch.DrawLine(Vector2.Zero, Vector2.One, Color.White);

    device.Received(1).DrawMesh(Arg.Any<Mesh<Vertex>>(), material, vertexCount: 2, indexCount: 0, MeshType.Lines);
  }

  [Test, AutoFixture]
  public void it_should_generate_a_valid_triangle_mesh(IGraphicsDevice device, Material material)
  {
    device.CreateBuffer<Vertex>().Returns(new InMemoryGraphicsBuffer<Vertex>());
    device.CreateBuffer<ushort>().Returns(new InMemoryGraphicsBuffer<ushort>());

    using var batch = new GeometryBatch(device);

    batch.Begin(material, Matrix4x4.Identity);
    batch.DrawSolidTriangle(Vector2.Zero, Vector2.One, Vector2.UnitX, Color.White);

    device.Received(1).DrawMesh(Arg.Any<Mesh<Vertex>>(), material, vertexCount: 3, indexCount: 0);
  }
}
