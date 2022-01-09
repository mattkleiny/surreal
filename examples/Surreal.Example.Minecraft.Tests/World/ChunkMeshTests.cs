using Minecraft.Worlds;

namespace Minecraft.World;

public class ChunkMeshTests
{
  [Test]
  public void it_should_generate_a_valid_chunk_mesh()
  {
    var chunk = Chunk.Generate(ChunkGenerators.Chaotic());
    var mesh  = new ChunkMesh(Substitute.For<IGraphicsDevice>(), chunk);

    Assert.DoesNotThrow(mesh.Invalidate);
  }
}
