using System.Runtime.CompilerServices;
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

  public ChunkSlice this[Vector3I offset, VolumeI size] => new(this, offset, size);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Block GetBlock(int x, int y, int z)
  {
    return Palette.GetBlock(GetVoxel(x, y, z));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void SetBlock(int x, int y, int z, Block value)
  {
    SetVoxel(x, y, z, Palette.GetId(value));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ushort GetVoxel(int x, int y, int z)
  {
    return Sample(x, y, z);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void SetVoxel(int x, int y, int z, ushort value, bool notifyChanged = true)
  {
    Sample(x, y, z) = value;

    if (notifyChanged)
    {
      Changed?.Invoke();
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void NotifyChanged()
  {
    Changed?.Invoke();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private ref ushort Sample(int x, int y, int z)
  {
    return ref voxels.Span[x + y * Width + z * Width * Height];
  }
}

/// <summary>A sub-slice of a larger <see cref="Chunk"/>.</summary>
public readonly struct ChunkSlice
{
  public static ChunkSlice Empty => default;

  private readonly Chunk?   chunk;
  private readonly Vector3I offset;
  private readonly VolumeI  size;

  public ChunkSlice(Chunk chunk, Vector3I offset, VolumeI size)
  {
    this.chunk  = chunk;
    this.offset = offset;
    this.size   = size;
  }

  public int Width  => size.Width;
  public int Height => size.Height;
  public int Depth  => size.Depth;

  public BlockPalette Palette => chunk?.Palette ?? Block.Palette;

  public ChunkSlice this[Vector3I offset, VolumeI size]
  {
    get
    {
      if (chunk == null)
      {
        return Empty;
      }

      return new ChunkSlice(chunk, this.offset + offset, size);
    }
  }

  public Span<ushort> Voxels
  {
    get
    {
      if (chunk == null)
      {
        return Span<ushort>.Empty;
      }

      var start = offset.X + offset.Y * Chunk.Size.Width + offset.Z * Chunk.Size.Width * Chunk.Size.Height;

      return chunk.Voxels[start..(start + size.Total)];
    }
  }

  public void NotifyChanged()
  {
    chunk?.NotifyChanged();
  }

  public static implicit operator ChunkSlice(Chunk chunk) => new(chunk, Vector3I.Zero, Chunk.Size);
}
