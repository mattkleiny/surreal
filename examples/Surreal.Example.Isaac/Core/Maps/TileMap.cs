using System;
using System.Numerics;
using Surreal.Collections;
using Surreal.Diagnostics.Profiling;
using Surreal.Framework.PathFinding;
using Surreal.Framework.Tiles;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics.Grids;
using Surreal.Mathematics.Linear;
using Surreal.Mathematics.Timing;

namespace Isaac.Core.Maps {
  public sealed class TileMap : IPathFindingGrid {
    private static readonly IProfiler   Profiler   = ProfilerFactory.GetProfiler<TileMap>();
    private static readonly TileLayer[] LayerOrder = {TileLayer.Background, TileLayer.Midground, TileLayer.Foreground};

    public TileMap(int width, int height) {
      Background = new(width, height, Tile.Palette);
      Midground  = new(width, height, Tile.Palette);
      Foreground = new(width, height, Tile.Palette);

      CollisionMask = new(Foreground, (_, _, tile) => tile.IsCollidable);
      PathingMask   = new(Background, (_, _, tile) => tile.IsPathable);

      foreach (var (x, y) in Foreground.EnumerateCells()) {
        if (x == 0 || y == 0 || x == Foreground.Width - 1 || y == Foreground.Height - 1) {
          Foreground[x, y] = Tile.Wall;
        }
      }
    }

    public TileMap<Tile> Background { get; }
    public TileMap<Tile> Midground  { get; }
    public TileMap<Tile> Foreground { get; }

    public TileMapMask<Tile> CollisionMask { get; }
    public TileMapMask<Tile> PathingMask   { get; }

    public Tile this[int x, int y, TileLayer layer] {
      get => this[layer][x, y];
      set => this[layer][x, y] = value;
    }

    public TileMap<Tile> this[TileLayer layer] => layer switch {
      TileLayer.Background => Background,
      TileLayer.Midground  => Midground,
      TileLayer.Foreground => Foreground,
      _                    => throw new ArgumentOutOfRangeException(nameof(layer)),
    };

    public void Draw(GeometryBatch batch, DeltaTime deltaTime) {
      using var _ = Profiler.Track(nameof(Draw));

      // draw the entire tile-map from bottom to top
      for (var i = 0; i < LayerOrder.Length; i++) {
        var layer = this[LayerOrder[i]];

        foreach (var (x, y) in layer.EnumerateCells()) {
          var tile = layer[x, y];
          if (tile.IsVisible) {
            batch.DrawSolidQuad(
                center: new Vector2(x * 16f, y * 16f),
                size: new Vector2(16f, 16f),
                color: tile.Color
            );
          }
        }
      }
    }

    void IPathFindingGrid.GetNeighbours(Vector2I position, ref SpanList<Vector2I> results) {
      foreach (var neighbour in position.GetMooreNeighbourhood()) {
        if (neighbour != position &&
            neighbour.X >= 0 && neighbour.X < CollisionMask.Width &&
            neighbour.Y >= 0 && neighbour.Y < CollisionMask.Height &&
            !CollisionMask[neighbour.X, neighbour.Y]) {
          results.Add(neighbour);
        }
      }
    }
  }
}