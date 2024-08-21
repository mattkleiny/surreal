using Surreal.Collections;
using Surreal.Collections.Slices;

namespace Surreal.Input.Collections;

/// <summary>
/// A <see cref="MultiDictionary{TKey,TValue}"/> wrapper with support for variable key types.
/// </summary>
internal sealed class VariableMultiDictionary<TValue>
{
  private readonly Dictionary<Type, object> _dictionaries = new();

  /// <summary>
  /// Attempts to get the values associated with the given key.
  /// </summary>
  public bool TryGetValues<TKey>(TKey key, out ReadOnlySlice<TValue> values)
    where TKey : notnull
  {
    if (!_dictionaries.TryGetValue(typeof(TKey), out var dictionary))
    {
      values = ReadOnlySlice<TValue>.Empty;
      return false;
    }

    var multiDictionary = (MultiDictionary<TKey, TValue>)dictionary;

    values = multiDictionary[key];
    return true;
  }

  /// <summary>
  /// Adds the given value to the dictionary.
  /// </summary>
  public void Add<TKey>(TKey key, TValue value)
    where TKey : notnull
  {
    if (!_dictionaries.TryGetValue(typeof(TKey), out var dictionary))
    {
      dictionary = new MultiDictionary<TKey, TValue>();

      _dictionaries.Add(typeof(TKey), dictionary);
    }

    var multiDictionary = (MultiDictionary<TKey, TValue>)dictionary;

    multiDictionary.Add(key, value);
  }

  /// <summary>
  /// Removes the given value from the dictionary.
  /// </summary>
  public void Remove<TKey>(TKey key, TValue value)
    where TKey : notnull
  {
    if (!_dictionaries.TryGetValue(typeof(TKey), out var dictionary))
    {
      return;
    }

    var multiDictionary = (MultiDictionary<TKey, TValue>)dictionary;

    multiDictionary.Remove(key, value);
  }

  /// <summary>
  /// Clears all dictionaries.
  /// </summary>
  public void Clear()
  {
    _dictionaries.Clear();
  }
}
