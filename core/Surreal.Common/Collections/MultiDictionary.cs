namespace Surreal.Collections;

/// <summary>
/// A dictionary with multiple values per key.
/// </summary>
public sealed class MultiDictionary<TKey, TValue>
  where TKey : notnull
{
  private readonly Dictionary<TKey, List<TValue>> _dictionary;

  public MultiDictionary()
    : this(EqualityComparer<TKey>.Default)
  {
  }

  public MultiDictionary(IEqualityComparer<TKey> comparer)
  {
    _dictionary = new Dictionary<TKey, List<TValue>>(comparer);
  }

  /// <summary>
  /// The total number of keys in the dictionary.
  /// </summary>
  public int KeyCount => _dictionary.Count;

  /// <summary>
  /// The total number of values in the dictionary.
  /// </summary>
  public int ValueCount
  {
    get
    {
      var count = 0;

      foreach (var value in _dictionary.Values)
      {
        count += value.Count;
      }

      return count;
    }
  }

  /// <summary>
  /// The keys of the dictionary.
  /// </summary>
  public IEnumerable<TKey> Keys => _dictionary.Keys;

  /// <summary>
  /// The values of the dictionary.
  /// </summary>
  public IEnumerable<TValue> Values => _dictionary.Values.SelectMany(_ => _);

  public ReadOnlySlice<TValue> this[TKey key]
  {
    get
    {
      if (_dictionary.TryGetValue(key, out var collection))
      {
        return collection;
      }

      return ReadOnlySlice<TValue>.Empty;
    }
  }

  public bool TryGetValues(TKey key, out ReadOnlySlice<TValue> result)
  {
    if (_dictionary.TryGetValue(key, out var collection))
    {
      result = collection;
      return true;
    }

    result = ReadOnlySlice<TValue>.Empty;
    return false;
  }

  public bool ContainsKey(TKey key)
  {
    return _dictionary.ContainsKey(key);
  }

  public void Add(TKey key, TValue value)
  {
    GetOrCreateList(key).Add(value);
  }

  public void AddRange(TKey key, IEnumerable<TValue> values)
  {
    GetOrCreateList(key).AddRange(values);
  }

  public void Remove(TKey key, TValue value)
  {
    if (_dictionary.TryGetValue(key, out var collection))
    {
      collection.Remove(value);

      // prune empty collections
      if (collection.Count == 0)
      {
        _dictionary.Remove(key);
      }
    }
  }

  public void RemoveAll(TKey key)
  {
    _dictionary.Remove(key);
  }

  public void Clear()
  {
    _dictionary.Clear();
  }

  private List<TValue> GetOrCreateList(TKey key)
  {
    if (!_dictionary.TryGetValue(key, out var list))
    {
      _dictionary[key] = list = new List<TValue>();
    }

    return list;
  }
}
