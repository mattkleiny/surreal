using System;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Minecraft.Worlds
{
  public readonly record struct Volume(int Width, int Height, int Depth)
  {
    public int Total => Width * Height * Depth;
  }

  public readonly record struct Voxel(ushort Id)
  {
    public static implicit operator Voxel(ushort id) => new(id);
  }

  public sealed record Block(Voxel Voxel)
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

    public static Block Dirt { get; } = new(0)
    {
      Name        = "Dirt",
      Description = "A block of dirt",
      Color       = Color.Yellow,
    };

    public static Block Grass { get; } = new(1)
    {
      Name        = "Grass",
      Description = "A block of grass",
      Color       = Color.Green,
    };
  }

  public sealed class Chunk
  {
    public static Volume Size { get; } = new(16, 128, 16);

    private readonly IBuffer<ushort> voxels = Buffers.AllocatePinned<ushort>(Size.Total);

    public Block this[int x, int y, int z]
    {
      get => throw new NotImplementedException();
      set => voxels.Data[x + y * Size.Width + z * Size.Width * Size.Height] = value.Voxel.Id;
    }
  }

  public sealed class Region
  {
  }
}
