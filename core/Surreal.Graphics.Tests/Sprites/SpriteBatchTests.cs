using Surreal.Graphics.Materials;
using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Graphics.Utilities;
using static Surreal.Graphics.Sprites.SpriteBatch;

namespace Surreal.Graphics.Sprites;

public class SpriteBatchTests
{
  [Test, AutoFixture]
  public void it_should_generate_a_valid_triangle_mesh(IGraphicsDevice device, Material material, Texture texture)
  {
    device.CreateBuffer<Vertex>().Returns(new InMemoryGraphicsBuffer<Vertex>());
    device.CreateBuffer<ushort>().Returns(new InMemoryGraphicsBuffer<ushort>());

    using var batch = new SpriteBatch(device, spriteCount: 1);

    batch.Begin(material, Matrix4x4.Identity);
    batch.Draw(texture, Vector2.Zero, new Vector2(16, 16));
    batch.Flush();

    device.Received(1).DrawMesh(Arg.Any<Mesh<Vertex>>(), material, vertexCount: 4, indexCount: 6, MeshType.Triangles);
  }
}
