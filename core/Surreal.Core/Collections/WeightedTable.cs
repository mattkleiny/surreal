using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Surreal.Mathematics;

namespace Surreal.Collections {
  public sealed class WeightedTable<T> : IEnumerable<T>
      where T : class {
    private readonly List<Entry> entries = new();

    public int Count => entries.Count;

    public void Add(T item, Normal weight) {
      entries.Add(new Entry(item, weight));
    }

    public T? Select(Seed seed = default) {
      var random = seed.ToRandom();
      var entry  = entries.SelectRandomlyWithWeight(random, _ => _.Weight);

      return entry?.Item;
    }

    public T? SelectWhere(Func<T, bool> condition, Seed seed = default) {
      var random = seed.ToRandom();
      var entry = entries
          .Where(_ => condition(_.Item))
          .SelectRandomlyWithWeight(random, _ => _.Weight);

      return entry?.Item;
    }

    public void Clear() {
      entries.Clear();
    }

    public Enumerator             GetEnumerator() => new(this);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<T> {
      private readonly WeightedTable<T> table;
      private          int              index;

      public Enumerator(WeightedTable<T> table)
          : this() {
        this.table = table;
        Reset();
      }

      public T           Current => table.entries[index].Item;
      object IEnumerator.Current => Current;

      public bool MoveNext() => ++index < table.Count;
      public void Reset()    => index = -1;

      public void Dispose() {
      }
    }

    private sealed class Entry {
      public readonly T      Item;
      public readonly Normal Weight;

      public Entry(T item, Normal weight) {
        Item   = item;
        Weight = weight;
      }
    }
  }
}