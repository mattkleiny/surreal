using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Surreal.Framework.Palettes {
  [DebuggerDisplay("Palette - {Count} entries")]
  public sealed class StaticPalette<T> : IPalette<T>
      where T : IHasId {
    private readonly Dictionary<ushort, T> tileById;
    private readonly Dictionary<T, ushort> idByTile;

    public StaticPalette() {
      // scan all static members of the given type and add to container
      var tiles = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
          .Where(field => typeof(T).IsAssignableFrom(field.FieldType))
          .Select(field => (T) field.GetValue(null))
          .ToList();

      tileById = tiles.ToDictionary(tile => tile.Id, tile => tile);
      idByTile = tiles.ToDictionary(tile => tile, tile => tile.Id);
    }

    public T   Empty => tileById[default];
    public int Count => tileById.Count;

    public ushort this[T tile] {
      get {
        if (!idByTile.TryGetValue(tile, out var id)) {
          throw new Exception($"The given value is not recognized: {tile}");
        }

        return id;
      }
    }

    public T this[ushort id] {
      get {
        if (!tileById.TryGetValue(id, out var tile)) {
          throw new Exception($"The given id is not recognized: {id}");
        }

        return tile;
      }
    }
  }
}