using Minecraft.Worlds;
using Surreal.Mathematics;

namespace Surreal.World;

public class ChunkTests
{
  [Test, Benchmark(ThresholdMs = 0.1f)]
  public void it_should_generate_a_valid_chunk()
  {
    var chunk = Chunk.Generate(ChunkGenerators.Solid(Block.Dirt));

    Assert.IsNotNull(chunk);
  }

  [Test]
  public void it_should_slice_into_the_middle_of_a_chunk()
  {
    var chunk = new Chunk();

    var slice       = chunk[new Vector3I(1, 1, 1), new VolumeI(2, 2, 2)];
    var sliceVoxels = slice.Voxels;

    for (ushort i = 0; i < sliceVoxels.Length; i++)
    {
      sliceVoxels[i] = i;
    }

    var subSlice       = slice[new Vector3I(1, 1, 1), new VolumeI(2, 2, 2)];
    var subSliceVoxels = subSlice.Voxels;

    for (ushort i = 0; i < subSliceVoxels.Length; i++)
    {
      subSliceVoxels[i] = i;
    }
  }
}
