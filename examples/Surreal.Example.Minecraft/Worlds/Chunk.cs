using System.Runtime.CompilerServices;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Minecraft.Worlds;

/// <summary>A chunk of the game world.</summary>
public sealed class Chunk
{
  public static VolumeI Size { get; } = new(16, 128, 16);

  private readonly IBuffer<ushort> voxels = Buffers.AllocatePinned<ushort>(Size.Total);

  /// <summary>Generates a <see cref="Chunk"/> with the given <see cref="ChunkGenerator"/>.</summary>
  public static Chunk Generate(ChunkGenerator generator)
  {
    var chunk = new Chunk();

    generator(chunk);

    return chunk;
  }

  public Chunk()
    : this(Block.Palette)
  {
  }

  public Chunk(BlockPalette palette)
  {
    Palette = palette;
  }

  /// <summary>Invoked when a voxel or block is changed in the chunk.</summary>
  public event Action? Changed;

  public int          Width   => Size.Width;
  public int          Height  => Size.Height;
  public int          Depth   => Size.Depth;
  public Span<ushort> Voxels  => voxels.Span;
  public BlockPalette Palette { get; }

  public Block GetBlock(int x, int y, int z)
  {
    return Palette.GetBlock(GetVoxel(x, y, z));
  }

  public void SetBlock(int x, int y, int z, Block value)
  {
    SetVoxel(x, y, z, Palette.GetId(value));
  }

  public ushort GetVoxel(int x, int y, int z)
  {
    return Sample(x, y, z);
  }

  public void SetVoxel(int x, int y, int z, ushort value, bool notifyChanged = true)
  {
    Sample(x, y, z) = value;

    if (notifyChanged)
    {
      Changed?.Invoke();
    }
  }

  public void NotifyChanged()
  {
    Changed?.Invoke();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private ref ushort Sample(int x, int y, int z)
  {
    return ref voxels.Span[x + y * Size.Width + z * Size.Width * Size.Height];
  }
}
