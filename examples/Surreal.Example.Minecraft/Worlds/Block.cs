using Surreal.Mathematics;

namespace Minecraft.Worlds;

public sealed record Block(ushort Id)
{
  private static uint   nextId = 0;
  private static ushort NextId() => (ushort) Interlocked.Increment(ref nextId);

  public string Name        { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
  public Color  Color       { get; init; } = Color.White;

  public static Block Air { get; } = new(NextId())
  {
    Name        = "Air",
    Description = "An empty space.",
    Color       = Color.Clear
  };

  public static Block Dirt { get; } = new(NextId())
  {
    Name        = "Dirt",
    Description = "A block of dirt",
    Color       = Color.Yellow
  };

  public static Block Grass { get; } = new(NextId())
  {
    Name        = "Grass",
    Description = "A block of grass",
    Color       = Color.Green
  };

  public static BlockPalette Palette { get; } = new(Air, Dirt, Grass);
}

public sealed class BlockPalette
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
