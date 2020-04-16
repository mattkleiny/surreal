using Surreal.Framework.Tiles;
using Xunit;

namespace Surreal.Jam.Tiles
{
  public class TileMapTests
  {
    [Fact]
    public void it_should_read_and_write_tiles()
    {
      var map = new TileMap<Tile>(512, 512, Tile.Palette)
      {
        [0, 0]     = Tile.Water,
        [128, 128] = Tile.Water,
        [511, 511] = Tile.Water
      };

      Assert.Equal(Tile.Water, map[0, 0]);
      Assert.Equal(Tile.Air, map[64, 64]);
      Assert.Equal(Tile.Water, map[128, 128]);
      Assert.Equal(Tile.Air, map[256, 256]);
      Assert.Equal(Tile.Water, map[511, 511]);
    }

    [Fact]
    public void it_should_fire_an_event_when_tiles_change()
    {
      var map     = new TileMap<Tile>(512, 512, Tile.Palette);
      var changed = false;

      map.TileChanged += (x, y, oldTile, newTile) => changed = true;

      map[0, 0] = Tile.Water;

      Assert.True(changed);
    }
  }
}