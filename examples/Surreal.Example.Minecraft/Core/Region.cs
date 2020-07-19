using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Minecraft.Core.Coordinates;
using Minecraft.Core.Generation;
using Surreal.Framework;
using Surreal.Framework.Palettes;
using Surreal.IO;

namespace Minecraft.Core {
  public sealed class Region : IDisposable {
    private readonly IPalette<Block> palette;
    private readonly ChunkGenerator  chunkGenerator;

    private readonly Dictionary<ChunkPos, Chunk> chunks;
    private readonly IBuffer<ushort>             buffer;

    public Region(IBuffer<ushort> buffer, IPalette<Block> palette, ChunkGenerator chunkGenerator, Biome biome) {
      this.buffer         = buffer;
      this.palette        = palette;
      this.chunkGenerator = chunkGenerator;

      chunks = new Dictionary<ChunkPos, Chunk>(World.ChunksPerRegion.Total);

      Biome = biome;
    }

    public Biome Biome { get; }

    public Chunk GetChunk(ChunkPos position) {
      if (!chunks.TryGetValue(position, out var chunk)) {
        chunk = chunks[position] = CreateChunk(position);
      }

      return chunk;
    }

    private Chunk CreateChunk(ChunkPos position) {
      // take a slice of the region's data
      var offset = Math.Abs(position.X + position.Y + position.Z) * World.VoxelsPerChunk.Total;
      var slice  = buffer.Slice(offset, World.VoxelsPerChunk.Total);

      var chunk = new Chunk(slice, palette, chunkGenerator, position);

      Task.Run(() => chunk.Regenerate()); // build the initial chunk shape

      return chunk;
    }

    public void Dispose() {
      if (buffer is IDisposableBuffer<ushort> disposable) {
        disposable.Dispose();
      }
    }
  }
}