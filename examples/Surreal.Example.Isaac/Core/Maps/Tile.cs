using Surreal.Framework.Palettes;

namespace Isaac.Core.Maps {
  public sealed class Tile : IHasId {
    public static readonly Tile Empty = new Tile(id: 0);

    public static IPalette<Tile> Palette { get; } = new StaticPalette<Tile>();

    private Tile(ushort id) => Id = id;

    public ushort Id           { get; }
    public bool   IsCollidable { get; set; }
    public bool   IsPathable   { get; set; }
  }
}