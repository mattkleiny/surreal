using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Framework.Tiles.Importers;
using Surreal.IO;
using Surreal.Mathematics.Grids;

namespace Surreal.Framework.Tiles {
  [DebuggerDisplay("{Width}x{Height} tile map")]
  public class TileMap<TTile> : IGrid<TTile> {
    public delegate void TileChangedEvent(int x, int y, TTile oldTile, TTile newTile);

    private readonly DenseGrid<ushort> tiles;

    public TileMap(int width, int height, ITilePalette<TTile> palette)
        : this(width, height, palette, palette[0]) {
    }

    public TileMap(int width, int height, ITilePalette<TTile> palette, TTile defaultTile) {
      Palette = palette;

      tiles = new DenseGrid<ushort>(width, height);
      tiles.Fill(palette[defaultTile]);
    }

    public ITilePalette<TTile> Palette { get; }

    public int Width  => tiles.Width;
    public int Height => tiles.Height;

    public event TileChangedEvent TileChanged = null!;

    public TTile this[int x, int y] {
      get => Palette[tiles[x, y]];
      set {
        var oldTile = Palette[tiles[x, y]]!;
        if (!oldTile.Equals(value)) {
          tiles[x, y] = Palette[value];
          TileChanged?.Invoke(x, y, oldTile, value);
        }
      }
    }

    public sealed class TmxLoader : AssetLoader<TileMap<TTile>> {
      public delegate TTile TileConverter(uint gid);

      private readonly ITilePalette<TTile> palette;
      private readonly TileConverter       converter;
      private readonly TTile               defaultTile;

      public TmxLoader(ITilePalette<TTile> palette, TileConverter converter, TTile defaultTile) {
        this.palette     = palette;
        this.converter   = converter;
        this.defaultTile = defaultTile;
      }

      public override async Task<TileMap<TTile>> LoadAsync(Path path, IAssetLoaderContext context) {
        await using var stream = await path.OpenInputStreamAsync();

        var input  = TmxDocument.Load(stream);
        var output = new TileMap<TTile>(input.Width, input.Height, palette, defaultTile);

        for (var i = 0; i < input.Layers.Count; i++) {
          var layer = input.Layers[i];
          if (layer.IsVisible != 1) continue;

          var parsed = layer.Data!.Decode().ToArray();

          for (var j = 0; j < parsed.Length; j++) {
            var x = j % layer.Width;
            var y = j / layer.Width;

            var gid  = parsed[j] & (uint) ~FlipFlags.All;
            var tile = converter(gid);

            output[x, y] = tile;
          }
        }

        return output;
      }
    }
  }
}