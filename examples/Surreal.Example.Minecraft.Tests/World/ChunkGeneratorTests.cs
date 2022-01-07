using Minecraft.Worlds;

namespace Surreal.World;

public class ChunkGeneratorTests
{
  [Test]
  public void it_should_generate_a_solid_chunk()
  {
    Assert.IsNotNull(Chunk.Generate(ChunkGenerators.Solid(Block.Dirt)));
  }

  [Test]
  public void it_should_generate_a_flat_chunk()
  {
    Assert.IsNotNull(Chunk.Generate(ChunkGenerators.Flat(Block.Dirt, height: Chunk.Size.Height / 2)));
  }

  [Test]
  public void it_should_generate_a_chaotic_chunk()
  {
    Assert.IsNotNull(Chunk.Generate(ChunkGenerators.Chaotic()));
  }
}
