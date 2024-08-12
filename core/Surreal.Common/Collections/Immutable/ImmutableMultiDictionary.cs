namespace Surreal.Collections.Immutable;

/// <summary>
/// An immutable dictionary that allows multiple values for a single key.
/// </summary>
public sealed class ImmutableMultiDictionary<TKey, TValue>
  where TKey : notnull
{
  /// <summary>
  /// An empty multi dictionary.
  /// </summary>
  public static ImmutableMultiDictionary<TKey, TValue> Empty { get; } = new(ImmutableDictionary<TKey, ImmutableArray<TValue>>.Empty);

  private readonly ImmutableDictionary<TKey, ImmutableArray<TValue>> _entries;

  private ImmutableMultiDictionary(ImmutableDictionary<TKey, ImmutableArray<TValue>> entries)
  {
    _entries = entries;
  }

  /// <summary>
  /// The total number of keys in the dictionary.
  /// </summary>
  public int KeyCount => _entries.Count;

  /// <summary>
  /// The total number of values in the dictionary.
  /// </summary>
  public int ValueCount
  {
    get
    {
      var count = 0;

      foreach (var value in _entries.Values)
      {
        count += value.Length;
      }

      return count;
    }
  }

  /// <summary>
  /// Gets the number of keys in the dictionary.
  /// </summary>
  public ImmutableArray<TValue> this[TKey key]
  {
    get
    {
      if (!_entries.TryGetValue(key, out var values))
      {
        return ImmutableArray<TValue>.Empty;
      }

      return values;
    }
  }

  /// <summary>
  /// Determines whether the dictionary contains a key.
  /// </summary>
  /// <param name="key"></param>
  /// <returns></returns>
  public bool ContainsKey(TKey key)
  {
    return _entries.ContainsKey(key);
  }

  /// <summary>
  /// Attempts to get the values for a key.
  /// </summary>
  public bool TryGetValues(TKey key, out ImmutableArray<TValue> values)
  {
    return _entries.TryGetValue(key, out values);
  }

  /// <summary>
  /// Adds a value to the dictionary.
  /// </summary>
  [Pure]
  public ImmutableMultiDictionary<TKey, TValue> Add(TKey key, TValue value)
  {
    if (_entries.TryGetValue(key, out var values))
    {
      var result = _entries.SetItem(key, values.Add(value));

      return new ImmutableMultiDictionary<TKey, TValue>(result);
    }

    return new ImmutableMultiDictionary<TKey, TValue>(_entries.Add(key, [value]));
  }


  /// <summary>
  /// Adds multiple values to the dictionary.
  /// </summary>
  [Pure]
  public ImmutableMultiDictionary<TKey, TValue> AddRange(TKey key, IEnumerable<TValue> values)
  {
    if (!_entries.TryGetValue(key, out var existing))
    {
      return new ImmutableMultiDictionary<TKey, TValue>(_entries.SetItem(key, [..values]));
    }

    return new ImmutableMultiDictionary<TKey, TValue>(_entries.SetItem(key, existing.AddRange(values)));
  }

  /// <summary>
  /// Removes a single value for a key.
  /// </summary>
  [Pure]
  public ImmutableMultiDictionary<TKey, TValue> Remove(TKey key, TValue value)
  {
    if (!_entries.TryGetValue(key, out var values))
    {
      return this;
    }

    var result = values.Remove(value);
    if (result.Length == 0)
    {
      return new ImmutableMultiDictionary<TKey, TValue>(_entries.Remove(key));
    }

    return new ImmutableMultiDictionary<TKey, TValue>(_entries.SetItem(key, result));
  }

  /// <summary>
  /// Removes all values for a key.
  /// </summary>
  [Pure]
  public ImmutableMultiDictionary<TKey, TValue> RemoveAll(TKey key)
  {
    if (!_entries.TryGetValue(key, out _))
    {
      return this;
    }

    return new ImmutableMultiDictionary<TKey, TValue>(_entries.Remove(key));
  }

  /// <summary>
  /// Clears the dictionary.
  /// </summary>
  [Pure]
  public ImmutableMultiDictionary<TKey, TValue> Clear()
  {
    return new ImmutableMultiDictionary<TKey, TValue>(ImmutableDictionary<TKey, ImmutableArray<TValue>>.Empty);
  }

  /// <summary>
  /// A builder for <see cref="ImmutableMultiDictionary{TKey, TValue}"/>s.
  /// </summary>
  public sealed class Builder(IEqualityComparer<TKey>? keyComparer = null)
  {
    private readonly ImmutableDictionary<TKey, ImmutableArray<TValue>>.Builder _builder = ImmutableDictionary.CreateBuilder<TKey, ImmutableArray<TValue>>(keyComparer);

    /// <summary>
    /// Adds a new item to the list.
    /// </summary>
    public void Add(TKey key, TValue value)
    {
      if (_builder.TryGetValue(key, out var values))
      {
        _builder[key] = values.Add(value);
      }
      else
      {
        _builder.Add(key, [value]);
      }
    }

    /// <summary>
    /// Finalizes the builder into an <see cref="ImmutableMultiDictionary{TKey, TValue}"/>.
    /// </summary>
    public ImmutableMultiDictionary<TKey, TValue> ToImmutable()
    {
      return new ImmutableMultiDictionary<TKey, TValue>(_builder.ToImmutable());
    }
  }
}

/// <summary>
/// Helpers for creating immutable multi-dictionaries.
/// </summary>
public static class ImmutableMultiDictionary
{
  /// <summary>
  /// Creates a builder for <see cref="ImmutableMultiDictionary{TKey, TValue}"/>s.
  /// </summary>
  public static ImmutableMultiDictionary<TKey, TValue>.Builder CreateBuilder<TKey, TValue>(IEqualityComparer<TKey>? keyComparer = null)
    where TKey : notnull
  {
    return new ImmutableMultiDictionary<TKey, TValue>.Builder(keyComparer);
  }

  /// <summary>
  /// Converts a sequence of key-value pairs into an immutable multi-dictionary.
  /// </summary>
  public static ImmutableMultiDictionary<TKey, T> ToImmutableMultiDictionary<T, TKey>(
    this IEnumerable<T> source,
    Func<T, TKey> keySelector,
    IEqualityComparer<TKey>? keyComparer = null)
    where TKey : notnull
  {
    return ToImmutableMultiDictionary<T, TKey, T>(source, keySelector, it => it, keyComparer);
  }

  /// <summary>
  /// Converts a sequence of key-value pairs into an immutable multi-dictionary.
  /// </summary>
  public static ImmutableMultiDictionary<TKey, TValue> ToImmutableMultiDictionary<T, TKey, TValue>(
    this IEnumerable<T> source,
    Func<T, TKey> keySelector,
    Func<T, TValue> valueSelector,
    IEqualityComparer<TKey>? keyComparer = null)
    where TKey : notnull
  {
    var builder = CreateBuilder<TKey, TValue>(keyComparer);

    foreach (var item in source)
    {
      var key = keySelector(item);
      var value = valueSelector(item);

      builder.Add(key, value);
    }

    return builder.ToImmutable();
  }
}
