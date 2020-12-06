using System.Diagnostics;
using Surreal.Framework.Palettes;
using Surreal.Graphics;

namespace Prelude.Core {
  [DebuggerDisplay("{Id} - {Name}")]
  public sealed record Tile(ushort Id, string Name, bool IsSolid, Color Color, string? Texture = null) : IHasId {
    public static readonly Tile Void  = new(Id: 0, Name: "Void", IsSolid: false, Color.Black);
    public static readonly Tile Floor = new(Id: 1, Name: "Floor", IsSolid: false, Color.Black);
    public static readonly Tile Wall  = new(Id: 2, Name: "Wall", IsSolid: true, Color.White) {Texture = "textures_0"};

    public static readonly IPalette<Tile> Palette = new StaticPalette<Tile>();
  }
}