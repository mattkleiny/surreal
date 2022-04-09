using Minecraft.Worlds;

namespace Minecraft.World;

public class ChunkGeneratorTests
{
  [Test]
  public void it_should_generate_a_solid_chunk()
  {
    Chunk.Generate(ChunkGenerators.Solid(Block.Dirt)).Should().NotBeNull();
  }

  [Test]
  public void it_should_generate_a_flat_chunk()
  {
    Chunk.Generate(ChunkGenerators.Flat(Block.Dirt, height: Chunk.Size.Y / 2)).Should().NotBeNull();
  }

  [Test]
  public void it_should_generate_a_chaotic_chunk()
  {
    Chunk.Generate(ChunkGenerators.Chaotic()).Should().NotBeNull();
  }
}
