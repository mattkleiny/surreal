using System.Numerics;
using Surreal.Graphics;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics.Linear;

namespace Isaac.Core {
  public enum RoomType {
    Standard,
    Start,
    Boss,
    Item,
    Shop,
  }

  public sealed class Room {
    public static readonly Vector2 Size = new Vector2(15, 9);

    public Vector2I Position    = Vector2I.Zero;
    public DoorMask NormalDoors = DoorMask.None;
    public DoorMask SecretDoors = DoorMask.None;
    public RoomType Type        = RoomType.Standard;

    public void DrawGizmos(GeometryBatch batch) {
      var position = new Vector2(Position.X * Size.X, Position.Y * Size.Y);

      batch.DrawWireQuad(
          center: position,
          size: Size,
          color: Color.White
      );

      DrawDoor(position, batch, NormalDoors, Directions.North, Color.Green);
      DrawDoor(position, batch, NormalDoors, Directions.East, Color.Green);
      DrawDoor(position, batch, NormalDoors, Directions.South, Color.Green);
      DrawDoor(position, batch, NormalDoors, Directions.West, Color.Green);

      DrawDoor(position, batch, SecretDoors, Directions.North, Color.Blue);
      DrawDoor(position, batch, SecretDoors, Directions.East, Color.Blue);
      DrawDoor(position, batch, SecretDoors, Directions.South, Color.Blue);
      DrawDoor(position, batch, SecretDoors, Directions.West, Color.Blue);

      static void DrawDoor(Vector2 center, GeometryBatch batch, DoorMask mask, Directions direction, Color color) {
        if (!mask.HasDoor(direction)) return;

        center = direction switch {
            Directions.North => new Vector2(center.X, center.Y + Size.Y / 2f),
            Directions.South => new Vector2(center.X, center.Y - Size.Y / 2f),
            Directions.East  => new Vector2(center.X + Size.X / 2f, center.Y),
            Directions.West  => new Vector2(center.X - Size.X / 2f, center.Y),
            _                => Vector2.Zero
        };

        batch.DrawCircle(center, 1f, color);
      }
    }
  }
}