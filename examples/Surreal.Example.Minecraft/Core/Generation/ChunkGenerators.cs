using System.Threading.Tasks;
using Surreal.Mathematics;
using Surreal.Mathematics.Linear;

namespace Minecraft.Core.Generation {
  public delegate void ChunkGenerator(IChunkView chunk);

  public static class ChunkGenerators {
    public static ChunkGenerator Flat(int height, Block block) => chunk => {
      for (var z = 0; z < chunk.Depth; z++)
      for (var y = 0; y < chunk.Height; y++)
      for (var x = 0; x < chunk.Width; x++) {
        if (y >= height) {
          chunk[x, y, z] = Block.Air;
        }
        else {
          chunk[x, y, z] = block;
        }
      }
    };

    public static ChunkGenerator Chaotic(Seed seed = default) => chunk => {
      var random = seed.ToRandom();

      for (var z = 0; z < chunk.Depth; z++)
      for (var y = 0; y < chunk.Height; y++)
      for (var x = 0; x < chunk.Width; x++) {
        chunk[x, y, z] = Block.Palette[(ushort) random.Next(1, 3)];
      }
    };

    public static ChunkGenerator Striated(params (int, int, Block)[] levels) => chunk => {
      for (var z = 0; z < chunk.Depth; z++)
      for (var y = 0; y < chunk.Height; y++)
      for (var x = 0; x < chunk.Width; x++)
      for (var i = 0; i < levels.Length; i++) {
        var (start, end, block) = levels[i];

        if (y >= start && y <= end) {
          chunk[x, y, z] = block;
        }
      }
    };

    public static ChunkGenerator Partitioned(ChunkGenerator generator) {
      return Partitioned(generator, maxDepth: 1, World.VoxelsPerChunk / 8);
    }

    public static ChunkGenerator Partitioned(ChunkGenerator generator, int maxDepth, Volume subVolume) => view => {
      var chunk = (Chunk) view;

      Parallel.ForEach(
          chunk.Partition(maxDepth, subVolume),
          slice => generator(slice)
      );
    };
  }
}