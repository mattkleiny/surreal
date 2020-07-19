using Isaac.Core.Dungeons;
using Surreal.Mathematics;
using Surreal.Mathematics.Linear;
using Surreal.Mathematics.Timing;

namespace Isaac.Core {
  public sealed class Dungeon : Actor {
    public Floor Floor { get; } = new Floor();

    public Dungeon(Seed seed = default) {
      var room = Floor[0, 0] = new Room {
          Type = RoomType.Start
      };

      room.AddRoom(Direction.North)
          .AddRoom(Direction.North, RoomType.Item)
          .AddRoom(Direction.East)
          .AddRoom(Direction.East)
          .AddRoom(Direction.South, RoomType.Boss)
          .AddRoom(Direction.West, RoomType.Secret);
    }

    public override void Draw(DeltaTime deltaTime) {
      base.Draw(deltaTime);

      // TODO: support arbitrary transforms for wireframe geometry?
      Floor.DrawGizmos(Game.Current.GeometryBatch);
    }
  }
}