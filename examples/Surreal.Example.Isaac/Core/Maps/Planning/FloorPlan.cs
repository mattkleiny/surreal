using System;
using Surreal.Collections;
using Surreal.Framework.PathFinding;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics.Grids;
using Surreal.Mathematics.Linear;

namespace Isaac.Core.Maps.Planning {
  public sealed class FloorPlan : IPathFindingGrid {
    private readonly SparseGrid<RoomPlan> rooms = new SparseGrid<RoomPlan>(room => room.Position);

    public RoomPlan? this[int x, int y] {
      get => this[new Vector2I(x, y)];
      set => this[new Vector2I(x, y)] = value;
    }

    public RoomPlan? this[Vector2I position] {
      get => rooms[position];
      set {
        rooms[position] = value;

        if (value != null) {
          value.Floor    = this;
          value.Position = position;
        }
      }
    }

    public void Add(RoomPlan room) {
      if (rooms[room.Position] != null) {
        throw new Exception($"The cell at {room.Position} is already occupied!");
      }

      this[room.Position] = room;
    }

    public void DrawGizmos(GeometryBatch batch) {
      foreach (var room in rooms) {
        room.DrawGizmos(batch);
      }
    }

    void IPathFindingGrid.GetNeighbours(Vector2I position, ref SpanList<Vector2I> results) {
      var room = this[position];
      if (room != null) {
        foreach (var direction in room.NormalDoors) {
          var adjacent = position + direction.ToVector2I();
          if (this[adjacent] != null) {
            results.Add(adjacent);
          }
        }
      }
    }
  }
}