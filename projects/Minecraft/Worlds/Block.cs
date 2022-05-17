using System.Runtime.CompilerServices;

namespace Minecraft.Worlds;

/// <summary>A single block with metadata.</summary>
public sealed record Block(ushort Id) : Enumeration<Block>
{
  private static uint nextId = 0;
  private static ushort NextId() => (ushort) Interlocked.Increment(ref nextId);

  public string Name    { get; init; } = string.Empty;
  public Color  Color   { get; init; } = Color.White;
  public bool   IsSolid { get; init; } = true;

  public static Block Air { get; } = new(NextId())
  {
    Name = "Air",
    Color = Color.Clear,
    IsSolid = false,
  };

  public static Block Dirt { get; } = new(NextId())
  {
    Name = "Dirt",
    Color = Color.Yellow,
  };

  public static Block Grass { get; } = new(NextId())
  {
    Name = "Grass",
    Color = Color.Green,
  };

  public static BlockPalette Palette { get; } = new(All);
}

/// <summary>A palette of <see cref="Block"/>s for fast lookup.</summary>
public sealed class BlockPalette
{
  private readonly Dictionary<ushort, Block> blocksById;
  private readonly Dictionary<Block, ushort> idsByBlock;

  public BlockPalette(IEnumerable<Block> blocks)
  {
    blocksById = blocks.ToDictionary(_ => _.Id, _ => _);
    idsByBlock = blocks.ToDictionary(_ => _, _ => _.Id);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Block GetBlock(ushort id) => blocksById[id];

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public ushort GetId(Block block) => idsByBlock[block];
}
