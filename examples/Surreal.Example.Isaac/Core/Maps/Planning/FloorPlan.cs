using System;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics.Grids;
using Surreal.Mathematics.Linear;

namespace Isaac.Core.Maps.Planning {
  public sealed class FloorPlan {
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
  }
}