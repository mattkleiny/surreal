using System;
using System.Collections;
using Surreal.Mathematics.Grids;

namespace Surreal.Framework.Tiles {
  public sealed class TileMapMask<TTile> : IGrid<bool>, IDisposable {
    public delegate bool TileSelector(int x, int y, TTile tile);

    private readonly BitArray       mask;
    private readonly TileMap<TTile> map;
    private readonly TileSelector   selector;

    public TileMapMask(TileMap<TTile> map, TileSelector selector) {
      this.map      = map;
      this.selector = selector;

      map.TileChanged += OnTileChanged;

      mask = new BitArray(map.Width * map.Height);

      Invalidate();
    }

    public int Width  => map.Width;
    public int Height => map.Height;

    public bool this[int x, int y] {
      get => mask[x + y * Width];
      set => mask[x + y * Width] = value;
    }

    public void Invalidate() {
      for (var y = 0; y < Height; y++)
      for (var x = 0; x < Width; x++) {
        var tile = map[x, y];

        mask[x + y * Width] = selector(x, y, tile);
      }
    }

    private void OnTileChanged(int x, int y, TTile oldTile, TTile newTile) {
      mask[x + y * Width] = selector(x, y, newTile);
    }

    public void Dispose() {
      map.TileChanged -= OnTileChanged;
    }
  }
}