using Surreal.Graphics.Meshes;
using Surreal.Mathematics.Grids;
using Surreal.Mathematics.Linear;

namespace Isaac.Core {
  public sealed class Floor {
    private readonly SparseGrid<Room> rooms = new SparseGrid<Room>(room => room.Position);

    public Room? this[int x, int y] {
      get => this[new Vector2I(x, y)];
      set => this[new Vector2I(x, y)] = value;
    }

    public Room? this[Vector2I position] {
      get => rooms[position];
      set {
        rooms[position] = value;

        if (value != null) {
          value.Position = position;
        }
      }
    }

    public void DrawGizmos(GeometryBatch batch) {
      foreach (var room in rooms) {
        room.DrawGizmos(batch);
      }
    }
  }
}