using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Surreal;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Mathematics;

namespace Isaac.Core.Maps
{
  internal sealed class RoomCatalogue
  {
    private readonly List<RoomPrefab> prefabs;

    public RoomCatalogue(IEnumerable<RoomPrefab> prefabs)
    {
      this.prefabs = prefabs.ToList();
    }

    public RoomPrefab SelectPrefab(FloorType floorType, RoomType roomType, Directions doors)
    {
      var prefab = prefabs.FirstOrDefault(_ =>
        _.FloorType == floorType &&
        _.RoomType == roomType &&
        _.Doors == doors
      );

      if (prefab == null)
      {
        throw new Exception(
          $"Unable to locate a valid prefab of type {roomType} on floor {floorType} with doors <{doors.ToPermutationString()}>"
        );
      }

      return prefab;
    }

    public sealed class Loader : AssetLoader<RoomCatalogue>
    {
      public override async Task<RoomCatalogue> LoadAsync(Path path, IAssetLoaderContext context)
      {
        var paths = await path.EnumerateAsync("*.tmx");
        var maps  = paths.Select(context.GetAsync<RoomPrefab>).ToArray();

        await Task.WhenAll(maps);

        return new RoomCatalogue(maps.Select(_ => _.Result).ToArray());
      }
    }
  }
}