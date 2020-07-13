using System.Diagnostics;
using Surreal.Collections;
using Surreal.Framework.PathFinding;
using Surreal.Mathematics;
using Surreal.Mathematics.Grids;
using Surreal.Mathematics.Linear;

namespace Isaac.Core.Maps {
  [DebuggerDisplay("{Width}x{Height} {Type} floor")]
  public sealed class Floor : IGrid<Room?>, IPathFindingGrid {
    private readonly Grid<Room?> rooms;

    public Floor(FloorType type, int width, int height) {
      Type  = type;
      rooms = new Grid<Room?>(width, height, defaultValue: null);
    }

    public FloorType Type { get; }

    public int Width  => rooms.Width;
    public int Height => rooms.Height;

    public Room? this[int x, int y] {
      get => rooms[x, y];
      set => rooms[x, y] = value;
    }

    float IPathFindingGrid.GetCost(Vector2I from, Vector2I to) {
      return 1f;
    }

    void IPathFindingGrid.GetNeighbours(Vector2I position, ref SpanList<Vector2I> results) {
      foreach (var neighbour in position.GetVonNeumannNeighbourhood()) {
        if (neighbour != position && rooms.TryGet(neighbour.X, neighbour.Y) != null) {
          // TODO: make sure there is a door in the given direction

          results.Add(neighbour);
        }
      }
    }
  }
}