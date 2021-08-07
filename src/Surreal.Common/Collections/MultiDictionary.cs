using System.Collections.Generic;

namespace Surreal.Collections
{
  public sealed class MultiDictionary<TKey, TValue>
      where TKey : notnull
  {
    private readonly Dictionary<TKey, List<TValue>> dictionary;

    public MultiDictionary()
        : this(EqualityComparer<TKey>.Default)
    {
    }

    public MultiDictionary(IEqualityComparer<TKey> comparer)
    {
      dictionary = new Dictionary<TKey, List<TValue>>(comparer);
    }

    public int               Count => dictionary.Count;
    public IEnumerable<TKey> Keys  => dictionary.Keys;

    public ReadOnlySlice<TValue> this[TKey key]
    {
      get
      {
        if (dictionary.TryGetValue(key, out var collection))
        {
          return collection;
        }

        return ReadOnlySlice<TValue>.Empty;
      }
    }

    public bool TryGetValues(TKey key, out ReadOnlySlice<TValue> result)
    {
      if (dictionary.TryGetValue(key, out var collection))
      {
        result = collection;
        return true;
      }

      result = ReadOnlySlice<TValue>.Empty;
      return false;
    }

    public bool ContainsKey(TKey key)
    {
      return dictionary.ContainsKey(key);
    }

    public void Add(TKey key, TValue value)
    {
      dictionary.GetOrCreate(key).Add(value);
    }

    public void AddRange(TKey key, IEnumerable<TValue> values)
    {
      dictionary.GetOrCreate(key).AddRange(values);
    }

    public void Remove(TKey key, TValue value)
    {
      if (dictionary.TryGetValue(key, out var collection))
      {
        collection.Remove(value);

        // prune empty collections
        if (collection.Count == 0)
        {
          dictionary.Remove(key);
        }
      }
    }

    public void RemoveAll(TKey key)
    {
      dictionary.Remove(key);
    }

    public void Clear()
    {
      dictionary.Clear();
    }
  }
}