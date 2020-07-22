using Surreal.Framework.Palettes;

namespace Isaac.Core.Maps {
  public sealed class Tile : IHasId {
    public static readonly Tile Empty = new Tile(id: 0);

    public static IPalette<Tile> Palette { get; } = new StaticPalette<Tile>();

    public Tile(ushort id, bool isWalkable = false) {
      Id         = id;
      IsWalkable = isWalkable;
    }

    public ushort Id         { get; }
    public bool   IsWalkable { get; }
  }
}