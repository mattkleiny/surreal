using System.Collections.Generic;
using System.Numerics;
using Surreal.Graphics;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics.Linear;
using Surreal.Utilities;

namespace Isaac.Core.Maps.Planning {
  public sealed class RoomPlan {
    public static readonly Vector2 Size = new Vector2(15, 9);

    public FloorPlan?     Floor       = null;
    public RoomPlan?      Parent      = null;
    public List<RoomPlan> Children    = new List<RoomPlan>();
    public Vector2I       Position    = Vector2I.Zero;
    public DoorMask       NormalDoors = DoorMask.None;
    public DoorMask       SecretDoors = DoorMask.None;
    public RoomType       Type        = RoomType.Standard;

    public bool TryAddRoom(out RoomPlan room, Direction direction, RoomType type = RoomType.Standard) {
      if (Floor?[Position + direction.ToVector2I()] != null) {
        room = this;
        return false;
      }

      room = AddRoom(direction, type);
      return true;
    }

    public RoomPlan AddRoom(Direction direction, RoomType type = RoomType.Standard) {
      var room = new RoomPlan {
          Parent   = this,
          Position = Position + direction.ToVector2I(),
          Type     = type,
      };

      Children.Add(room);

      if (type == RoomType.Secret) {
        SecretDoors.Add(direction);
        room.SecretDoors.Add(direction.Opposite());
      } else {
        NormalDoors.Add(direction);
        room.NormalDoors.Add(direction.Opposite());
      }

      Floor?.Add(room);

      return room;
    }

    public void DrawGizmos(GeometryBatch batch) {
      var center = new Vector2(Position.X * Size.X, Position.Y * Size.Y);

      batch.DrawWireQuad(center, Size, Color.White);
      batch.DrawWireQuad(center, new Vector2(Size.X - 2f, Size.Y - 2f), Type switch {
          RoomType.Standard => Color.White,
          RoomType.Start    => Color.Green,
          RoomType.Boss     => Color.Red,
          RoomType.Item     => Color.Yellow,
          RoomType.Shop     => Color.Yellow,
          RoomType.Secret   => Color.Blue,
          _                 => Color.Clear
      });

      foreach (var direction in NormalDoors.UsedDoors.GetMaskValues()) {
        DrawDoor(center, batch, direction, Color.Green);
      }

      foreach (var direction in SecretDoors.UsedDoors.GetMaskValues()) {
        DrawDoor(center, batch, direction, Color.Blue);
      }

      static void DrawDoor(Vector2 center, GeometryBatch batch, Direction direction, Color color) {
        center = direction switch {
            Direction.North => new Vector2(center.X, center.Y + Size.Y / 2f),
            Direction.South => new Vector2(center.X, center.Y - Size.Y / 2f),
            Direction.East  => new Vector2(center.X + Size.X / 2f, center.Y),
            Direction.West  => new Vector2(center.X - Size.X / 2f, center.Y),
            _               => Vector2.Zero
        };

        batch.DrawCircle(center, 1f, color);
      }
    }
  }
}