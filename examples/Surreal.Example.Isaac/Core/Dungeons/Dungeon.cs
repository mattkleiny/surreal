using System;
using Surreal.Mathematics;
using Surreal.Mathematics.Linear;
using Surreal.Mathematics.Timing;
using Surreal.Utilities;

namespace Isaac.Core.Dungeons {
  public sealed class Dungeon : Actor {
    public Floor Floor { get; } = new Floor();

    public override void Draw(DeltaTime deltaTime) {
      base.Draw(deltaTime);

      // TODO: support arbitrary transforms for wireframe geometry?
      Floor.DrawGizmos(Game.Current.GeometryBatch);
    }
  }

  public delegate Dungeon DungeonGenerator(Seed seed = default);

  public static class DungeonGenerators {
    public static DungeonGenerator Fixed() => Factory((dungeon, random) => {
      var room = dungeon.Floor[0, 0] = new Room {
          Type = RoomType.Start
      };

      room.AddRoom(Direction.North)
          .AddRoom(Direction.East)
          .AddRoom(Direction.East)
          .AddRoom(Direction.South)
          .AddRoom(Direction.South, RoomType.Item)
          .AddRoom(Direction.West, RoomType.Boss)
          .AddRoom(Direction.North, RoomType.Secret);
    });

    public static DungeonGenerator Standard(IntRange rooms) => Factory((dungeon, random) => {
      var room = dungeon.Floor[0, 0] = new Room {
          Type = RoomType.Start
      };

      for (var i = 0; i < random.NextRange(rooms); i++) {
        generate:
        if (!room.TryAddRoom(out room, room.NormalDoors.UnusedDoors.SelectRandomly(random))) {
          goto generate;
        }
      }

      room.Type = RoomType.Boss;
    });

    private static DungeonGenerator Factory(Action<Dungeon, Random> factory) => seed => {
      var random  = seed.ToRandom();
      var dungeon = new Dungeon();

      factory(dungeon, random);

      return dungeon;
    };
  }
}