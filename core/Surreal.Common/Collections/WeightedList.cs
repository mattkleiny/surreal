namespace Surreal.Collections;

/// <summary>A list of weighted elements of <see cref="T"/>.</summary>
public sealed class WeightedList<T> : IEnumerable<T>
  where T : notnull
{
  private readonly List<Entry> entries = new();
  private float totalWeight = 0f;

  /// <summary>Adds a new item to the list.</summary>
  public void Add(T item, float weight = 1f)
  {
    totalWeight += weight;

    entries.Add(new Entry(item, totalWeight));
  }

  /// <summary>Clears all items from the list.</summary>
  public void Clear()
  {
    totalWeight = 0f;

    entries.Clear();
  }

  /// <summary>Selects an item from the list, or returns the given <see cref="defaultValue"/>.</summary>
  public T SelectOrDefault(T defaultValue = default!)
  {
    return SelectOrDefault(Random.Shared, defaultValue);
  }

  /// <summary>Selects an item from the list, or returns the given <see cref="defaultValue"/>.</summary>
  public T SelectOrDefault(Random random, T defaultValue = default!)
  {
    if (!TrySelect(random, out var result))
    {
      return defaultValue;
    }

    return result;
  }

  /// <summary>Attempts to select an item from the list, honoring random weights.</summary>
  public bool TrySelect([NotNullWhen(true)] out T? result)
  {
    return TrySelect(Random.Shared, out result);
  }

  /// <summary>Attempts to select an item from the list, honoring random weights.</summary>
  public bool TrySelect(Random random, [NotNullWhen(true)] out T? result)
  {
    var weight = random.NextDouble() * totalWeight;

    for (var i = 0; i < entries.Count; i++)
    {
      var entry = entries[i];
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

  /// <summary>A single entry in the <see cref="WeightedList{T}"/>.</summary>
  private readonly record struct Entry(T Item, float Weight);

  /// <summary>Allows enumerating active <see cref="T"/>s.</summary>
  public struct Enumerator : IEnumerator<T>
  {
    private readonly WeightedList<T> list;
    private List<Entry>.Enumerator enumerator;

    public Enumerator(WeightedList<T> list)
      : this()
    {
      this.list = list;
      Reset();
    }

    public T Current => enumerator.Current.Item;
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      return enumerator.MoveNext();
    }

    public void Reset()
    {
      enumerator = list.entries.GetEnumerator();
    }

    public void Dispose()
    {
      // no-op
    }
  }
}
