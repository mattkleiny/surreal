using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Framework.Tiles.Importers;
using Surreal.IO;
using Surreal.Mathematics;

namespace Isaac.Core.Maps {
  [DebuggerDisplay("{Width}x{Height} {RoomType} on {FloorType}")]
  internal sealed class RoomPrefab {
    private readonly TmxDocument document;

    public RoomPrefab(TmxDocument document) {
      this.document = document;

      FloorType = ParseEnum<FloorType>(document, "FloorType");
      RoomType  = ParseEnum<RoomType>(document, "RoomType");
      Doors     = ParseDirections(document);
    }

    public int Width  => document.Width;
    public int Height => document.Height;

    public FloorType  FloorType { get; }
    public RoomType   RoomType  { get; }
    public Directions Doors     { get; }

    public Room Instantiate(Seed seed = default) {
      var room = new Room(Width, Height, RoomType);

      for (var i = 0; i < document.Layers.Count; i++) {
        var layer = document.Layers[i];
        if (layer.IsVisible != 1) continue;

        var parsed = layer.Data.Decode().ToArray();

        for (var j = 0; j < parsed.Length; j++) {
          var x = j % layer.Width;
          var y = j / layer.Width;

          var gid  = parsed[j] & (uint) ~FlipFlags.All;
          var tile = Tile.Palette[(ushort) gid];

          room[x, y] = tile;
        }
      }

      return room;
    }

    private static Directions ParseDirections(TmxDocument document) {
      bool TryParseBool(string key) => document[key]?.Value == "true";

      var result = Directions.None;

      if (TryParseBool("HasNorthExit")) result |= Directions.North;
      if (TryParseBool("HasSouthExit")) result |= Directions.South;
      if (TryParseBool("HasEastExit")) result  |= Directions.East;
      if (TryParseBool("HasWestExit")) result  |= Directions.West;

      return result;
    }

    private static TEnum ParseEnum<TEnum>(TmxDocument document, string key)
        where TEnum : struct, Enum {
      var property = document[key];

      if (property == null || !Enum.TryParse<TEnum>(property.Value, out var result)) {
        throw new Exception($"Unable to parse {typeof(TEnum)} from room prefab!");
      }

      return result;
    }

    public sealed class Loader : AssetLoader<RoomPrefab> {
      public override async Task<RoomPrefab> LoadAsync(Path path, IAssetLoaderContext context) {
        await using var stream = await path.OpenInputStreamAsync();

        return new RoomPrefab(TmxDocument.Load(stream));
      }
    }
  }
}