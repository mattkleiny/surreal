using System;
using System.Collections.Generic;
using System.IO;
using Minecraft.Core.Coordinates;
using Minecraft.Core.Generation;
using Surreal.Framework;
using Surreal.Framework.Palettes;
using Surreal.IO;
using Surreal.Mathematics.Linear;

namespace Minecraft.Core {
  public sealed class World : IDisposable {
    public static readonly Volume ChunksPerRegion = new Volume(32, 1, 32);
    public static readonly Volume VoxelsPerChunk  = new Volume(16, 128, 16);
    public static readonly Volume VoxelsPerRegion = ChunksPerRegion * VoxelsPerChunk;

    private readonly IRegionStrategy strategy;
    private          Neighborhood    neighborhood;

    public static World CreateFinite(Volume regionsPerWorld, IPalette<Block> palette, ChunkGenerator chunkGenerator, BiomeSelector biomeSelector) {
      return new World(new FixedRegionStrategy(regionsPerWorld, palette, chunkGenerator, biomeSelector));
    }

    public static World CreateInfinite(string basePath, IPalette<Block> palette, ChunkGenerator chunkGenerator, BiomeSelector biomeSelector) {
      return new World(new StreamingRegionStrategy(basePath, palette, chunkGenerator, biomeSelector));
    }

    private World(IRegionStrategy strategy) => this.strategy = strategy;

    public Neighborhood Neighborhood {
      get => neighborhood;
      set {
        neighborhood = value;
        strategy.UpdateNeighborhood(in value);
      }
    }

    public Region? GetRegion(WorldPos position) => strategy.GetRegion(position.RegionPos);
    public Chunk?  GetChunk(WorldPos position)  => GetRegion(position)?.GetChunk(position.ChunkPos);

    public IEnumerable<Chunk> GetChunksInNeighborhood() {
      return GetChunksInNeighborhood(Neighborhood);
    }

    public IEnumerable<Chunk> GetChunksInNeighborhood(Neighborhood neighborhood) {
      foreach (var position in neighborhood) {
        var chunk = GetChunk(position);
        if (chunk != null) {
          yield return chunk;
        }
      }
    }

    public void Dispose() {
      strategy.Dispose();
    }

    private interface IRegionStrategy : IDisposable {
      Region? GetRegion(RegionPos position);
      void    UpdateNeighborhood(in Neighborhood neighborhood);
    }

    private sealed class FixedRegionStrategy : IRegionStrategy {
      private readonly Volume                    regionsPerWorld;
      private readonly IDisposableBuffer<ushort> buffer;
      private readonly Region[,,]                regions;

      public FixedRegionStrategy(Volume regionsPerWorld, IPalette<Block> palette, ChunkGenerator chunkGenerator, BiomeSelector biomeSelector) {
        this.regionsPerWorld = regionsPerWorld;

        // open an off-heap representation of the entire world's data and share it amongst all child regions
        buffer = Buffers.AllocateOffHeap<ushort>((regionsPerWorld * VoxelsPerRegion).Total);

        regions = new Region[regionsPerWorld.Width, regionsPerWorld.Height, regionsPerWorld.Depth];

        for (var z = 0; z < regionsPerWorld.Depth; z++)
        for (var y = 0; y < regionsPerWorld.Height; y++)
        for (var x = 0; x < regionsPerWorld.Width; x++) {
          // take a slice of the world's data
          var offset = x + y + z * VoxelsPerRegion.Total;
          var slice  = buffer.Slice(offset, VoxelsPerRegion.Total);

          var biome = biomeSelector(new RegionPos(x, y, z));

          regions[x, y, z] = new Region(slice, palette, chunkGenerator, biome);
        }
      }

      public Region? GetRegion(RegionPos position) {
        var (x, y, z) = position;

        if (x >= 0 && x < regionsPerWorld.Width &&
            y >= 0 && y < regionsPerWorld.Height &&
            z >= 0 && z < regionsPerWorld.Depth) {
          return regions[x, y, z];
        }

        return null;
      }

      public void UpdateNeighborhood(in Neighborhood neighborhood) {
      }

      public void Dispose() => buffer.Dispose();
    }

    private sealed class StreamingRegionStrategy : IRegionStrategy {
      private readonly Dictionary<RegionPos, Region> regions = new Dictionary<RegionPos, Region>();

      private readonly string          basePath;
      private readonly IPalette<Block> palette;
      private readonly ChunkGenerator  chunkGenerator;
      private readonly BiomeSelector   biomeSelector;

      public StreamingRegionStrategy(string basePath, IPalette<Block> palette, ChunkGenerator chunkGenerator, BiomeSelector biomeSelector) {
        this.basePath       = basePath;
        this.palette        = palette;
        this.chunkGenerator = chunkGenerator;
        this.biomeSelector  = biomeSelector;

        // make sure the base directory is available
        if (!Directory.Exists(basePath)) {
          Directory.CreateDirectory(basePath);
        }
      }

      public Region GetRegion(RegionPos position) {
        if (!regions.TryGetValue(position, out var region)) {
          var (x, y, z) = position;

          var path  = $"{basePath}/{x}_{y}_{z}.region";
          var biome = biomeSelector(position);

          // open a memory mapped view of the region's data and share it amongst all child chunks instances
          var buffer = Buffers.MapFromFile<ushort>(path, 0, VoxelsPerRegion.Total);

          region = regions[position] = new Region(buffer, palette, chunkGenerator, biome);
        }

        return region;
      }

      public void UpdateNeighborhood(in Neighborhood neighborhood) {
        // TODO: remove regions that now exist outside the new neighborhood
      }

      public void Dispose() {
        foreach (var region in regions.Values) {
          region.Dispose();
        }

        regions.Clear();
      }
    }
  }
}