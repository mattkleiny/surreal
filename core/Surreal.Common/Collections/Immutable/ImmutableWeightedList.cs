namespace Surreal.Collections.Immutable;

/// <summary>
/// An immutable list of weighted elements of <see cref="T" />.
/// </summary>
public sealed class ImmutableWeightedList<T> : IEnumerable<T>
  where T : notnull
{
  private readonly ImmutableList<Entry> _entries;
  private float _totalWeight;

  private ImmutableWeightedList(ImmutableList<Entry> entries, float totalWeight)
  {
    _entries = entries;
    _totalWeight = totalWeight;
  }

  /// <summary>
  /// The number of items in the list.
  /// </summary>
  public int Count => _entries.Count;

  /// <summary>
  /// Adds a new item to the list.
  /// </summary>
  [Pure]
  public ImmutableWeightedList<T> Add(T item, float weight = 1f)
  {
    return new ImmutableWeightedList<T>(
      _entries.Add(new Entry(item, _totalWeight + weight)),
      _totalWeight + weight
    );
  }

  /// <summary>
  /// Clears all items from the list.
  /// </summary>
  [Pure]
  public ImmutableWeightedList<T> Clear()
  {
    return new ImmutableWeightedList<T>(
      _entries.Clear(),
      _totalWeight = 0f
    );
  }

  /// <summary>
  /// Selects an item from the list, or returns the given <see cref="defaultValue" />.
  /// </summary>
  public T SelectOrDefault(T defaultValue = default!)
  {
    return SelectOrDefault(Random.Shared, defaultValue);
  }

  /// <summary>
  /// Selects an item from the list, or returns the given <see cref="defaultValue" />.
  /// </summary>
  public T SelectOrDefault(Random random, T defaultValue = default!)
  {
    if (!TrySelect(random, out var result))
    {
      return defaultValue;
    }

    return result;
  }

  /// <summary>
  /// Attempts to select an item from the list, honoring random weights.
  /// </summary>
  public bool TrySelect([NotNullWhen(true)] out T? result)
  {
    return TrySelect(Random.Shared, out result);
  }

  /// <summary>
  /// Attempts to select an item from the list, honoring random weights.
  /// </summary>
  public bool TrySelect(Random random, [NotNullWhen(true)] out T? result)
  {
    var weight = random.NextDouble() * _totalWeight;

    for (var i = 0; i < _entries.Count; i++)
    {
      var entry = _entries[i];
      if (entry.Weight >= weight)
      {
        result = entry.Item;
        return true;
      }
    }

    result = default;
    return false;
  }

  public Enumerator GetEnumerator()
  {
    return new Enumerator(this);
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  /// <summary>
  /// Allows enumerating active <see cref="T" />s.
  /// </summary>
  public struct Enumerator(ImmutableWeightedList<T> list) : IEnumerator<T>
  {
    private ImmutableList<Entry>.Enumerator _enumerator = list._entries.GetEnumerator();

    public T Current => _enumerator.Current.Item;
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      return _enumerator.MoveNext();
    }

    public void Reset()
    {
      _enumerator.Reset();
    }

    public void Dispose()
    {
      _enumerator.Dispose();
    }
  }

  /// <summary>
  /// A single entry in the <see cref="ImmutableWeightedList{T}" />.
  /// </summary>
  private readonly record struct Entry(T Item, float Weight);

  /// <summary>
  /// A builder for <see cref="ImmutableWeightedList{T}"/>s.
  /// </summary>
  public sealed class Builder
  {
    private readonly ImmutableList<Entry>.Builder _builder = ImmutableList.CreateBuilder<Entry>();
    private float _totalWeight;

    /// <summary>
    /// Adds a new item to the list.
    /// </summary>
    public void Add(T item, float weight = 1f)
    {
      _totalWeight += weight;
      _builder.Add(new Entry(item, weight));
    }

    /// <summary>
    /// Finalizes the builder into an <see cref="ImmutableWeightedList{T}"/>.
    /// </summary>
    public ImmutableWeightedList<T> ToImmutable()
    {
      return new ImmutableWeightedList<T>(_builder.ToImmutable(), _totalWeight);
    }
  }
}

/// <summary>
/// Static helpers for <see cref="ImmutableWeightedList{T}"/>s.
/// </summary>
public static class ImmutableWeightedList
{
  /// <summary>
  /// Creates a builder for <see cref="ImmutableWeightedList{T}"/>s.
  /// </summary>
  public static ImmutableWeightedList<T>.Builder CreateBuilder<T>()
    where T : notnull
  {
    return new ImmutableWeightedList<T>.Builder();
  }

  /// <summary>
  /// Creates a new <see cref="ImmutableWeightedList{T}"/> from the given <paramref name="elements"/>.
  /// </summary>
  public static ImmutableWeightedList<T> Create<T>(params (T, float)[] elements)
    where T : notnull
  {
    var builder = CreateBuilder<T>();

    foreach (var (element, weight) in elements)
    {
      builder.Add(element, weight);
    }

    return builder.ToImmutable();
  }
}
