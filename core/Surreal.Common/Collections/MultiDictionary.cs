using Surreal.Collections.Slices;

namespace Surreal.Collections;

/// <summary>
/// A dictionary with multiple values per key.
/// </summary>
public sealed class MultiDictionary<TKey, TValue>(IEqualityComparer<TKey> comparer)
  where TKey : notnull
{
  private readonly Dictionary<TKey, List<TValue>> _dictionary = new(comparer);

  public MultiDictionary()
    : this(EqualityComparer<TKey>.Default)
  {
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

  /// <summary>
  /// Gets the values associated with the given key.
  /// </summary>
  public ReadOnlySlice<TValue> this[TKey key]
  {
    get
    {
      if (!_dictionary.TryGetValue(key, out var collection))
      {
        return ReadOnlySlice<TValue>.Empty;
      }

      return collection;
    }
  }

  /// <summary>
  /// Gets the values associated with the given key.
  /// </summary>
  public bool TryGetValues(TKey key, out ReadOnlySlice<TValue> result)
  {
    if (!_dictionary.TryGetValue(key, out var collection))
    {
      result = ReadOnlySlice<TValue>.Empty;
      return false;
    }

    result = collection;
    return true;
  }

  /// <summary>
  /// Determines if the dictionary contains the given key.
  /// </summary>
  public bool ContainsKey(TKey key)
  {
    return _dictionary.ContainsKey(key);
  }

  /// <summary>
  /// Adds a new value to the dictionary.
  /// </summary>
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

  /// <summary>
  /// Removes all values associated with the given key.
  /// </summary>
  public void RemoveAll(TKey key)
  {
    _dictionary.Remove(key);
  }

  /// <summary>
  /// Clears the dictionary.
  /// </summary>
  public void Clear()
  {
    _dictionary.Clear();
  }

  private List<TValue> GetOrCreateList(TKey key)
  {
    if (!_dictionary.TryGetValue(key, out var list))
    {
      _dictionary[key] = list = [];
    }

    return list;
  }
}
