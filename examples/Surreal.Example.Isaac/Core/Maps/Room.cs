using System.Diagnostics;
using Surreal.Collections;
using Surreal.Framework.PathFinding;
using Surreal.Framework.Tiles;
using Surreal.Mathematics;
using Surreal.Mathematics.Grids;
using Surreal.Mathematics.Linear;

namespace Isaac.Core.Maps {
  [DebuggerDisplay("{Width}x{Height} {Type} room")]
  public sealed class Room : IGrid<Tile>, IPathFindingGrid {
    private readonly TileMap<Tile>     tiles;
    private readonly TileMapMask<Tile> collisionMask;
    private readonly TileMapMask<Tile> transitionMask;

    public Room(Room other)
        : this(other.tiles, other.Type) {
    }

    public Room(TileMap<Tile> tiles, RoomType type)
        : this(tiles.Width, tiles.Height, type) {
      tiles.Blit(tiles, 0, 0, tiles.Width, tiles.Height);
    }

    public Room(int width, int height, RoomType type) {
      Type = type;

      tiles = new TileMap<Tile>(width, height, Tile.Palette, defaultTile: Tile.Void);

      collisionMask  = new TileMapMask<Tile>(tiles, (x, y, tile) => tile.IsSolid);
      transitionMask = new TileMapMask<Tile>(tiles, (x, y, tile) => tile.IsDoor);
    }

    public RoomType   Type  { get; }
    public Directions Doors { get; set; } = Directions.All;

    public int Width  => tiles.Width;
    public int Height => tiles.Height;

    public Tile this[int x, int y] {
      get => tiles[x, y];
      set => tiles[x, y] = value;
    }

    public bool IsCollidable(int x, int y) => collisionMask[x, y];
    public bool IsTransition(int x, int y) => transitionMask[x, y];

    public bool HasDoor(Directions direction) => (Doors & direction) == direction;

    float IPathFindingGrid.GetCost(Vector2I from, Vector2I to) {
      return tiles.TryGet(to.X, to.Y, Tile.Void).Cost;
    }

    void IPathFindingGrid.GetNeighbours(Vector2I position, ref SpanList<Vector2I> results) {
      foreach (var neighbour in position.GetMooreNeighbourhood()) {
        if (neighbour != position && !tiles.TryGet(neighbour.X, neighbour.Y)?.IsSolid == true) {
          results.Add(neighbour);
        }
      }
    }
  }
}