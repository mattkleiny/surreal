using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Surreal.Framework.Palettes {
  [DebuggerDisplay("Palette - {Count} entries")]
  public sealed class StaticPalette<T> : IPalette<T>
      where T : IHasId {
    private readonly Dictionary<ushort, T> itemById;
    private readonly Dictionary<T, ushort> idByItem;

    public StaticPalette() {
      // scan all static members of the given type and add to container
      var items = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
          .Where(field => typeof(T).IsAssignableFrom(field.FieldType))
          .Select(field => (T) field.GetValue(null))
          .ToList();

      itemById = items.ToDictionary(item => item.Id, item => item);
      idByItem = items.ToDictionary(item => item, item => item.Id);
    }

    public T   Empty => itemById[default];
    public int Count => itemById.Count;

    public ushort this[T item] {
      get {
        if (!idByItem.TryGetValue(item, out var id)) {
          throw new Exception($"The given value is not recognized: {item}");
        }

        return id;
      }
    }

    public T this[ushort id] {
      get {
        if (!itemById.TryGetValue(id, out var item)) {
          throw new Exception($"The given id is not recognized: {id}");
        }

        return item;
      }
    }
  }
}