using System.Runtime.CompilerServices;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Minecraft.Worlds;

/// <summary>A chunk of the game world.</summary>
public sealed class Chunk
{
  public static Volume Size { get; } = new(16, 128, 16);

  private readonly IBuffer<ushort> voxels = Buffers.AllocatePinned<ushort>(Size.Total);
  private readonly BlockPalette    palette;

  public Chunk()
    : this(Block.Palette)
  {
  }

  public Chunk(BlockPalette palette)
  {
    this.palette = palette;
  }

  public Span<ushort> Voxels => voxels.Data;

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
