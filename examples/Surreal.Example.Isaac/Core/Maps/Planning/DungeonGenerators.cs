using System;
using System.Runtime.CompilerServices;
using Surreal.Diagnostics.Logging;
using Surreal.Mathematics;
using Surreal.Mathematics.Linear;
using Surreal.Utilities;

namespace Isaac.Core.Maps.Planning {
  public delegate Dungeon DungeonGenerator(Seed seed = default);

  public static class DungeonGenerators {
    private static readonly ILog Log = LogFactory.GetLog<DungeonGenerator>();

    public static DungeonGenerator Fixed() => Factory((dungeon, _) => {
      var room = dungeon.FloorPlan[0, 0] = new() {Type = RoomType.Start};

      room.AddRoom(Direction.North)
          .AddRoom(Direction.East)
          .AddRoom(Direction.East)
          .AddRoom(Direction.South)
          .AddRoom(Direction.South, RoomType.Item)
          .AddRoom(Direction.West, RoomType.Boss)
          .AddRoom(Direction.North, RoomType.Secret);
    });

    public static DungeonGenerator Standard(IntRange rooms) => Factory((dungeon, random) => {
      var room = dungeon.FloorPlan[0, 0] = new() {Type = RoomType.Start};

      for (var i = 0; i < random.NextRange(rooms); i++) {
        if (room.TryAddRoom(out room, room.NormalDoors.FreeDoors.SelectRandomly(random))) {
          i--; // keep trying until we get it right?
        }
      }

      room.Type = RoomType.Boss;
    });

    private static DungeonGenerator Factory(
        Action<Dungeon, Random> factory,
        [CallerMemberName] string? name = null) {
      return seed => Log.Profile($"Generated {name} dungeon", () => {
        var random  = seed.ToRandom();
        var dungeon = new Dungeon();

        factory(dungeon, random);

        return dungeon;
      });
    }
  }
}