using Surreal.Framework.Tiles;
using Xunit;

namespace Surreal.Jam.Tiles {
  public class TileMapMaskTests {
    [Fact]
    public void it_should_mask_values_on_tile_maps() {
      var map = new TileMap<Tile>(512, 512, Tile.Palette) {
          [0, 0]     = Tile.Water,
          [128, 128] = Tile.Water,
          [511, 511] = Tile.Water
      };

      var mask = new TileMapMask<Tile>(map, IsWaterTile);

      Assert.True(mask[0, 0]);
      Assert.False(mask[64, 64]);
      Assert.True(mask[128, 128]);
      Assert.False(mask[256, 256]);
      Assert.True(mask[511, 511]);
    }

    [Fact]
    public void it_should_stay_current_with_updating_map() {
      var map  = new TileMap<Tile>(512, 512, Tile.Palette);
      var mask = new TileMapMask<Tile>(map, IsWaterTile);

      Assert.False(mask[0, 0]);

      map[0, 0] = Tile.Water;

      Assert.True(mask[0, 0]);
    }

    private static bool IsWaterTile(int x, int y, Tile tile) => tile == Tile.Water;
  }
}