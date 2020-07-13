using System.Diagnostics;
using Surreal.Framework;
using Surreal.Framework.Tiles;
using Surreal.Graphics;
using Surreal.Graphics.Raycasting;

namespace Prelude.Core {
  [DebuggerDisplay("{Id} - {Name}")]
  public sealed class Tile : IHasId, IRaycastAwareTile {
    public static readonly Tile Void  = new Tile(id: 0, name: "Void", isSolid: false, Color.Black);
    public static readonly Tile Empty = new Tile(id: 1, name: "Floor", isSolid: false, Color.Black);

    public static readonly Tile Wall = new Tile(id: 2, name: "Wall", isSolid: true, Color.White) {
        Texture = "textures_0"
    };

    public static readonly ITilePalette<Tile> Palette = new ReflectiveTilePalette<Tile>();

    public Tile(ushort id, string name, bool isSolid, Color color) {
      Id      = id;
      Name    = name;
      IsSolid = isSolid;
      Color   = color;
    }

    public ushort  Id      { get; }
    public string  Name    { get; }
    public bool    IsSolid { get; }
    public Color   Color   { get; }
    public string? Texture { get; private set; }
  }
}