using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Surreal.Framework.Tiles {
  [DebuggerDisplay("Tile palette - {Count} tiles")]
  public sealed class ReflectiveTilePalette<TTile> : ITilePalette<TTile>
      where TTile : IHasId {
    private readonly Dictionary<ushort, TTile> tileById;
    private readonly Dictionary<TTile, ushort> idByTile;

    public ReflectiveTilePalette() {
      // scan all static members of the given type and add to container
      var tiles = typeof(TTile).GetFields(BindingFlags.Public | BindingFlags.Static)
          .Where(field => typeof(TTile).IsAssignableFrom(field.FieldType))
          .Select(field => (TTile) field.GetValue(null))
          .ToList();

      tileById = tiles.ToDictionary(tile => tile.Id, tile => tile);
      idByTile = tiles.ToDictionary(tile => tile, tile => tile.Id);
    }

    public int Count => tileById.Count;

    public ushort this[TTile tile] {
      get {
        if (!idByTile.TryGetValue(tile, out var id)) {
          throw new Exception($"The given tile is not recognized: {tile}");
        }

        return id;
      }
    }

    public TTile this[ushort id] {
      get {
        if (!tileById.TryGetValue(id, out var tile)) {
          throw new Exception($"The given tile id is not recognized: {id}");
        }

        return tile;
      }
    }
  }
}