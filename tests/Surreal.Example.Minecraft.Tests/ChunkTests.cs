using Minecraft.Worlds;
using NUnit.Framework;

namespace Surreal.Example.Minecraft.Tests
{
  public sealed class ChunkTests
  {
    [Test]
    public void it_should_read_and_write_voxels()
    {
      var chunk = new Chunk();

      chunk.SetVoxel(15, 127, 15, 1000);

      Assert.AreEqual(1000, chunk.GetVoxel(15, 127, 15));
    }

    [Test]
    public void it_should_read_and_write_blocks()
    {
      var chunk = new Chunk();

      chunk.SetBlock(15, 127, 15, Block.Grass);

      Assert.AreEqual(Block.Grass, chunk.GetBlock(15, 127, 15));
    }
  }
}
