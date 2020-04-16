using Surreal;
using Surreal.Mathematics;
using Surreal.Mathematics.Grids;

namespace Isaac.Core.Maps
{
  internal static class FloorPlanner
  {
    public static Floor Plan(Seed seed, RoomCatalogue catalogue, FloorType floorType, int width, int height)
    {
      var random = seed.ToRandom();

      var plan  = new Grid<RoomType>(width, height, defaultValue: RoomType.Empty);
      var floor = new Floor(floorType, width, height);

      Directions EvaluateNeighbours(int x, int y)
      {
        var result = Directions.None;

        if (y < height - 1 && plan[x, y + 1] != RoomType.Empty) result |= Directions.North;
        if (y > 0 && plan[x, y - 1] != RoomType.Empty) result          |= Directions.South;
        if (y < width - 1 && plan[x + 1, y] != RoomType.Empty) result  |= Directions.East;
        if (x > 0 && plan[x - 1, y] != RoomType.Empty) result          |= Directions.West;

        return result;
      }

      // start from the center of the map and place the spawn room
      var midX = floor.Width / 2;
      var midY = floor.Height / 2;

      plan[midX, midY] = RoomType.Spawn;

      // TODO: trace a fair distance and place the item room
      // TODO: trace a fair distance and place the shop room
      // TODO: trace a fair distance and place the secret room
      // TODO: trace a fair distance and place the boss room
      // TODO: place filler rooms to fit corridors

      for (var y = 0; y < height; y++)
      for (var x = 0; x < width; x++)
      {
        var roomType = plan[x, y];
        if (roomType != RoomType.Empty)
        {
          var neighbours = EvaluateNeighbours(x, y);
          var prefab     = catalogue.SelectPrefab(floor.Type, roomType, Directions.All);

          floor[x, y] = prefab.Instantiate(random.NextSeed());
        }
      }

      return floor;
    }
  }
}
