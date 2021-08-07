using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Surreal.Mathematics;

namespace Surreal.Collections
{
  public sealed class WeightedList<T> : IEnumerable<T>
  {
    private readonly List<Entry> entries = new();

    public void Add(T item, float weight = 1f)
    {
      entries.Add(new Entry(item, weight));
    }

    public bool TrySelect(Seed seed, out T result)
    {
      return TrySelect(seed.ToRandom(), out result);
    }

    public bool TrySelect(Random random, out T result)
    {
      throw new NotImplementedException();
    }

    public bool TrySelectWhere(Func<T, bool> condition, Seed seed, out T result)
    {
      return TrySelectWhere(condition, seed.ToRandom(), out result);
    }

    public bool TrySelectWhere(Func<T, bool> condition, Random random, out T result)
    {
      throw new NotImplementedException();
    }

    public void Clear()
    {
      entries.Clear();
    }

    public IEnumerator<T>   GetEnumerator() => entries.Select(_ => _.Item).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private readonly struct Entry
    {
      public readonly T      Item;
      public readonly Normal Weight;

      public Entry(T item, Normal weight)
      {
        Item   = item;
        Weight = weight;
      }
    }
  }
}