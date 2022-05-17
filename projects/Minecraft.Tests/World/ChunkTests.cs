﻿using Minecraft.Worlds;

namespace Minecraft.World;

public class ChunkTests
{
  [Test]
  public void it_should_generate_a_valid_chunk()
  {
    var chunk = Chunk.Generate(ChunkGenerators.Solid(Block.Dirt));

    chunk.Should().NotBeNull();
  }

  [Test]
  public void it_should_slice_into_the_middle_of_a_chunk()
  {
    var chunk = new Chunk();

    var slice = chunk[new Point3(1, 1, 1), new Point3(2, 2, 2)];
    var sliceVoxels = slice.Voxels;

    for (ushort i = 0; i < sliceVoxels.Length; i++)
    {
      sliceVoxels[i] = i;
    }

    var subSlice = slice[new Point3(1, 1, 1), new Point3(2, 2, 2)];
    var subSliceVoxels = subSlice.Voxels;

    for (ushort i = 0; i < subSliceVoxels.Length; i++)
    {
      subSliceVoxels[i] = i;
    }
  }
}