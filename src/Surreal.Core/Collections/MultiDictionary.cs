using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Surreal.Collections
{
  public sealed class MultiDictionary<TKey, TValue> : IMultiDictionary<TKey, TValue>
  {
    private static readonly IReadOnlyList<TValue> EmptyList = new List<TValue>();

    private readonly Dictionary<TKey, List<TValue>> dictionary;

    public MultiDictionary()
      : this(EqualityComparer<TKey>.Default)
    {
    }

    public MultiDictionary(IEqualityComparer<TKey> comparer)
    {
      dictionary = new Dictionary<TKey, List<TValue>>(comparer);
    }

    public int                 Count  => dictionary.Count;
    public IEnumerable<TKey>   Keys   => dictionary.Keys;
    public IEnumerable<TValue> Values => dictionary.Values.SelectMany(collection => collection);

    public IReadOnlyList<TValue> this[TKey key]
    {
      get
      {
        if (ContainsKey(key))
        {
          return GetListForKey(key);
        }

        return EmptyList;
      }
    }

    public bool TryGetValues(TKey key, out IReadOnlyList<TValue> values)
    {
      if (dictionary.TryGetValue(key, out var result))
      {
        values = result;
        return true;
      }

      values = EmptyList;
      return false;
    }

    public void Add(TKey key, TValue value) => GetListForKey(key).Add(value);

    public void Remove(TKey key, TValue value)
    {
      var collection = GetListForKey(key);
      collection.Remove(value);

      // prune empty collections
      if (collection.Count == 0) dictionary.Remove(key);
    }

    public void RemoveAll(TKey key)   => dictionary.Remove(key);
    public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);
    public void Clear()               => dictionary.Clear();

    public IEnumerator<KeyValuePair<TKey, IReadOnlyList<TValue>>> GetEnumerator()
    {
      // fixes some cast weirdness in KeyValuePair
      foreach (var pair in dictionary)
      {
        yield return new KeyValuePair<TKey, IReadOnlyList<TValue>>(pair.Key, pair.Value);
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private List<TValue> GetListForKey(TKey key)
    {
      if (!dictionary.TryGetValue(key, out var list))
      {
        dictionary[key] = list = new List<TValue>();
      }

      return list;
    }
  }
}
