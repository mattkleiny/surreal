using Minecraft.Worlds;

namespace Surreal.World;

public class ChunkTests
{
  [Test]
  public void it_should_generate_a_valid_chunk()
  {
    var chunk = Chunk.Generate(ChunkGenerators.Solid(Block.Dirt));

    Assert.IsNotNull(chunk);
  }
}
