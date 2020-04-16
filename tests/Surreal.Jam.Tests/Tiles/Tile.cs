using Surreal.Framework;
using Surreal.Framework.Tiles;

namespace Surreal.Jam.Tiles
{
  internal sealed class Tile : IHasId
  {
    public static readonly Tile Air   = new Tile(id: 0);
    public static readonly Tile Water = new Tile(id: 1);

    public static readonly ITilePalette<Tile> Palette = new ReflectiveTilePalette<Tile>();

    public Tile(ushort id) => Id = id;

    public ushort Id { get; }
  }
}
