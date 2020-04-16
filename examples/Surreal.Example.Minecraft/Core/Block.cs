using System.Diagnostics;
using Surreal.Framework;
using Surreal.Framework.Voxels;
using Surreal.Graphics;

namespace Minecraft.Core
{
  [DebuggerDisplay("Block ID: {Id}, Name: {Name}")]
  public sealed class Block : IHasId
  {
    public static readonly Block Air = new Block(id: 0)
    {
      Name    = "Air",
      IsSolid = false
    };

    public static readonly Block Dirt = new Block(id: 1)
    {
      Name  = "Dirt",
      Color = Color.Brown
    };

    public static readonly Block Grass = new Block(id: 2)
    {
      Name  = "Grass",
      Color = Color.Green
    };

    public static readonly Block Stone = new Block(id: 3)
    {
      Name  = "Stone",
      Color = Color.Grey
    };

    public static readonly IVoxelPalette<Block> Palette = new ReflectiveVoxelPalette<Block>(typeof(Block));

    public ushort Id      { get; }
    public string Name    { get; private set; } = string.Empty;
    public bool   IsSolid { get; private set; } = true;
    public Color  Color   { get; private set; } = Color.White;

    public Block(ushort id) => Id = id;
  }
}
