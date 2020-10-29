using System.Diagnostics;
using Surreal.Framework.Palettes;
using Surreal.Graphics;

namespace Minecraft.Core {
  [DebuggerDisplay("Block ID: {Id}, Name: {Name}")]
  public sealed record Block(ushort Id, string Name, bool IsSolid, Color Color) : IHasId {
    public static readonly Block Air   = new Block(Id: 0, Name: "Air", IsSolid: false, Color: Color.White);
    public static readonly Block Dirt  = new Block(Id: 1, Name: "Dirt", IsSolid: true, Color: Color.Brown);
    public static readonly Block Grass = new Block(Id: 2, Name: "Grass", IsSolid: true, Color: Color.Green);
    public static readonly Block Stone = new Block(Id: 3, Name: "Stone", IsSolid: true, Color: Color.Grey);

    public static readonly IPalette<Block> Palette = new StaticPalette<Block>();
  }
}