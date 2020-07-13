using System.Diagnostics;
using Surreal.Framework;
using Surreal.Framework.Tiles;

namespace Isaac.Core.Maps {
  // TODO: support loading these from disk?

  [DebuggerDisplay("{Id} - {Name}")]
  public sealed class Tile : IHasId {
    private const float Impassable = float.MaxValue;

    public static readonly Tile Void = new Tile(id: 0, name: "Void", isSolid: false, cost: Impassable);

    public static readonly Tile Floor = new Tile(id: 10, name: "Floor", isSolid: false, cost: 1f) {
        Texture = "textures_9"
    };

    public static readonly Tile Wall = new Tile(id: 12, name: "Wall", isSolid: true, cost: Impassable) {
        Texture = "textures_11"
    };

    public static readonly ITilePalette<Tile> Palette = new ReflectiveTilePalette<Tile>();

    public Tile(ushort id, string name, bool isSolid, float cost, bool isDoor = false) {
      Id      = id;
      Name    = name;
      Cost    = cost;
      IsSolid = isSolid;
      IsDoor  = isDoor;
    }

    public ushort  Id      { get; }
    public string  Name    { get; }
    public float   Cost    { get; }
    public bool    IsSolid { get; }
    public bool    IsDoor  { get; }
    public string? Texture { get; private set; }
  }
}