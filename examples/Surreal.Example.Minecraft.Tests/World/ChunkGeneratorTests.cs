using Minecraft.Worlds;

namespace Minecraft.World;

public class ChunkGeneratorTests
{
  [Test]
  public void it_should_generate_a_solid_chunk()
  {
    var generator = ChunkGenerators.Solid(Block.Dirt);

    Chunk.Generate(generator).Should().NotBeNull();
  }

  [Test]
  public void it_should_generate_a_flat_chunk()
  {
    var generator = ChunkGenerators.Flat(Block.Dirt, height: Chunk.Size.Y / 2);

    Chunk.Generate(generator).Should().NotBeNull();
  }

  [Test]
  public void it_should_generate_a_chaotic_chunk()
  {
    var generator = ChunkGenerators.Chaotic();

    Chunk.Generate(generator).Should().NotBeNull();
  }
}
