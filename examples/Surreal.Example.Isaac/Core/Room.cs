using Surreal.Mathematics.Linear;

namespace Isaac.Core {
  public sealed class Room {
    public Vector2I Position = Vector2I.Zero;
    public DoorMask Doors    = DoorMask.None;
    public RoomType Type     = RoomType.Standard;

    public sealed class Plan {
      public Vector2I Position = Vector2I.Zero;
      public DoorMask Doors    = DoorMask.None;
      public RoomType Type     = RoomType.Standard;

      public Room ToRoom() => new Room {
          Position = Position,
          Doors    = Doors,
          Type     = Type
      };
    }
  }
}