using Minecraft.Core.Coordinates;
using Minecraft.Core.Generation;
using Surreal.Framework.Palettes;
using Surreal.IO;
using Surreal.Mathematics.Linear;
using Surreal.Mathematics.Tensors;

namespace Minecraft.Core {
  public sealed class Chunk : IChunkView {
    public delegate void BlockChangeHandler(int x, int y, int z, Block oldBlock, Block newBlock);

    private readonly IPalette<Block>  palette;
    private readonly ChunkGenerator   generator;
    private readonly Tensor3D<ushort> voxels;

    public Chunk(IBuffer<ushort> buffer, IPalette<Block> palette, ChunkGenerator generator, ChunkPos position) {
      this.palette   = palette;
      this.generator = generator;

      voxels = new Tensor3D<ushort>(buffer, World.VoxelsPerChunk.Width, World.VoxelsPerChunk.Height, World.VoxelsPerChunk.Depth);

      Position = position;

      Bounds = new AABB(
          position - new Vector3I(Width, Height, Depth) / 2,
          position + new Vector3I(Width, Height, Depth) / 2
      );
    }

    public event BlockChangeHandler BlockChanged = null!;

    public int Width  => voxels.Width;
    public int Height => voxels.Height;
    public int Depth  => voxels.Depth;

    public ChunkPos Position { get; }
    public AABB     Bounds   { get; }

    public void Regenerate() => generator(this);

    public ChunkSlice Slice(Vector3I offset, Volume dimensions) {
      return new ChunkSlice(this, offset, dimensions);
    }

    public Block this[int x, int y, int z] {
      get {
        if (x < 0 || x >= Width) return palette.Empty;
        if (y < 0 || y >= Height) return palette.Empty;
        if (z < 0 || z >= Depth) return palette.Empty;

        var voxel = voxels[x, y, z];

        return palette[voxel];
      }
      set {
        var voxel = voxels[x, y, z];
        var block = palette[voxel];

        voxels[x, y, z] = palette[value];

        if (value != block) {
          BlockChanged?.Invoke(x, y, z, block, value);
        }
      }
    }
  }
}