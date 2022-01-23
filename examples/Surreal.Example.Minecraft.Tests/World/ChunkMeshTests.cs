using Minecraft.Worlds;

namespace Minecraft.World;

public class ChunkMeshTests
{
  [Test, AutoFixture]
  public void it_should_generate_a_valid_chunk_mesh(IGraphicsServer server)
  {
    var chunk = Chunk.Generate(ChunkGenerators.Chaotic());
    var mesh  = new ChunkMesh(server, chunk);

    Assert.DoesNotThrow(mesh.Invalidate);
  }
}
