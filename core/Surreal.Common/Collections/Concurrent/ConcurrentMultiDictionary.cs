namespace Surreal.Collections.Concurrent;

/// <summary>
/// A concurrent version of <see cref="MultiDictionary{TKey,TValue}" />.
/// </summary>
public sealed class ConcurrentMultiDictionary<TKey, TValue>(IEqualityComparer<TKey> comparer)
  where TKey : notnull
{
  private readonly ConcurrentDictionary<TKey, ImmutableArray<TValue>> _dictionary = new(comparer);

  public ConcurrentMultiDictionary()
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
        count += value.Length;
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
  public ImmutableArray<TValue> this[TKey key]
  {
    get
    {
      if (!_dictionary.TryGetValue(key, out var collection))
      {
        return ImmutableArray<TValue>.Empty;
      }

      return collection;
    }
  }

  /// <summary>
  /// Gets the values associated with the given key.
  /// </summary>
  public bool TryGetValues(TKey key, out ImmutableArray<TValue> result)
  {
    return _dictionary.TryGetValue(key, out result);
  }

  /// <summary>
  /// Adds a value to the dictionary.
  /// </summary>
  public void Add(TKey key, TValue value)
  {
    _dictionary.AddOrUpdate(
      key: key,
      addValueFactory: _ => [value],
      updateValueFactory: (_, values) => values.Add(value)
    );
  }

  /// <summary>
  /// Adds a range of values to the dictionary.
  /// </summary>
  public void AddRange(TKey key, IEnumerable<TValue> values)
  {
    _dictionary.AddOrUpdate(
      key: key,
      addValueFactory: _ => [..values],
      updateValueFactory: (_, existing) => existing.AddRange(values)
    );
  }

  /// <summary>
  /// Removes the given value from the given key.
  /// </summary>
  public void Remove(TKey key, TValue value)
  {
    if (!_dictionary.TryGetValue(key, out var values))
    {
      return;
    }

    _dictionary.TryUpdate(
      key: key,
      newValue: values.Remove(value),
      comparisonValue: values
    );
  }

  /// <summary>
  /// Removes all values associated with the given key.
  /// </summary>
  public void RemoveAll(TKey key)
  {
    _dictionary.TryRemove(key, out _);
  }

  /// <summary>
  /// Clears the dictionary.
  /// </summary>
  public void Clear()
  {
    _dictionary.Clear();
  }
}
