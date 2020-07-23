using Surreal.Collections;
using Surreal.Framework.PathFinding;
using Surreal.Framework.Tiles;
using Surreal.Mathematics.Grids;
using Surreal.Mathematics.Linear;

namespace Isaac.Core.Maps {
  public sealed class TileMap : IPathFindingGrid {
    public TileMap(int width, int height) {
      Background = new TileMap<Tile>(width, height, Tile.Palette);
      Midground  = new TileMap<Tile>(width, height, Tile.Palette);
      Foreground = new TileMap<Tile>(width, height, Tile.Palette);

      CollisionMask = new TileMapMask<Tile>(Foreground, (x, y, tile) => !tile.IsWalkable);
    }

    public TileMap<Tile>     Background    { get; }
    public TileMap<Tile>     Midground     { get; }
    public TileMap<Tile>     Foreground    { get; }
    public TileMapMask<Tile> CollisionMask { get; }

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