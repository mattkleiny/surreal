using System.Collections.Generic;
using Surreal.Mathematics.Linear;

namespace Minecraft.Core.Generation {
  public static class ChunkPartitioner {
    public static IEnumerable<ChunkSlice> Partition(this Chunk chunk, int maxDepth, Volume subVolume) {
      // TODO: actually implement me

      yield return chunk.Slice(Vector3I.Zero, World.VoxelsPerChunk);
    }
  }
}