namespace Minecraft.Worlds;

/// <summary>Generates a chunk of the world, given a slice of it.</summary>
public delegate void ChunkGenerator(ChunkSlice slice);

/// <summary>Commonly used <see cref="ChunkGenerator"/>s.</summary>
public static class ChunkGenerators
{
  public static ChunkGenerator Solid(Block block) => chunk =>
  {
    chunk.Voxels.Fill(block.Id);

    chunk.NotifyChanged();
  };

  public static ChunkGenerator Flat(Block flatBlock, int height) => chunk =>
  {
    var voxels = chunk.Voxels;

    for (var z = 0; z < chunk.Depth; z++)
    for (var y = 0; y < chunk.Height; y++)
    for (var x = 0; x < chunk.Width; x++)
    {
      var block = y >= height ? Block.Air : flatBlock;

      voxels[x + y * chunk.Width + z * chunk.Width * chunk.Height] = block.Id;
    }

    chunk.NotifyChanged();
  };

  public static ChunkGenerator Chaotic(Seed seed = default) => chunk =>
  {
    var random = seed.ToRandom();
    var voxels = chunk.Voxels;

    for (var z = 0; z < chunk.Depth; z++)
    for (var y = 0; y < chunk.Height; y++)
    for (var x = 0; x < chunk.Width; x++)
    {
      var block = chunk.Palette.GetBlock((ushort) random.Next(1, 3));

      voxels[x + y * chunk.Width + z * chunk.Width * chunk.Height] = block.Id;
    }

    chunk.NotifyChanged();
  };
}
