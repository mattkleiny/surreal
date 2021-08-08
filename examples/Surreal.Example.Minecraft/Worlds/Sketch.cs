using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Minecraft.Worlds
{
  public readonly record struct Volume(int Width, int Height, int Depth)
  {
    public int Total => Width * Height * Depth;
  }

  public sealed record Block(ushort Id)
  {
    public string Name        { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Color  Color       { get; init; } = Color.White;

    public static Block Air { get; } = new(0)
    {
      Name        = "Air",
      Description = "An empty space.",
      Color       = Color.Clear,
    };

    public static Block Dirt { get; } = new(1)
    {
      Name        = "Dirt",
      Description = "A block of dirt",
      Color       = Color.Yellow,
    };

    public static Block Grass { get; } = new(2)
    {
      Name        = "Grass",
      Description = "A block of grass",
      Color       = Color.Green,
    };

    public static BlockPalette Palette { get; } = new(Air, Dirt, Grass);
  }

  public interface IBlockPalette
  {
    Block  GetBlock(ushort id);
    ushort GetId(Block block);
  }

  public sealed class BlockPalette : IBlockPalette
  {
    private readonly Dictionary<ushort, Block> blocksById;
    private readonly Dictionary<Block, ushort> idsByBlock;

    public BlockPalette(params Block[] blocks)
    {
      blocksById = blocks.ToDictionary(_ => _.Id, _ => _);
      idsByBlock = blocks.ToDictionary(_ => _, _ => _.Id);
    }

    public Block  GetBlock(ushort id) => blocksById[id];
    public ushort GetId(Block block)  => idsByBlock[block];
  }

  public sealed class Chunk
  {
    public static Volume Size { get; } = new(16, 128, 16);

    private readonly IBuffer<ushort> voxels = Buffers.AllocatePinned<ushort>(Size.Total);
    private readonly IBlockPalette   palette;

    public Chunk()
        : this(Block.Palette)
    {
    }

    public Chunk(IBlockPalette palette)
    {
      this.palette = palette;
    }

    public ushort GetVoxel(int x, int y, int z)               => Sample(x, y, z);
    public void   SetVoxel(int x, int y, int z, ushort value) => Sample(x, y, z) = value;

    public Block GetBlock(int x, int y, int z)              => palette.GetBlock(GetVoxel(x, y, z));
    public void  SetBlock(int x, int y, int z, Block value) => SetVoxel(x, y, z, palette.GetId(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref ushort Sample(int x, int y, int z)
    {
      return ref voxels.Data[x + y * Size.Width + z * Size.Width * Size.Height];
    }
  }

  public sealed class Region
  {
  }
}
