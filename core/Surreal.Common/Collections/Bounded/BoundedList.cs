namespace Surreal.Collections.Bounded;

/// <summary>
/// A <see cref="List{T}" /> with a fixed-sized upper bound.
/// </summary>
public sealed class BoundedList<T> : IEnumerable<T>
{
  private readonly List<T> _list;

  public BoundedList(int capacity = 0, int maxCapacity = 32)
  {
    Debug.Assert(capacity >= 0);
    Debug.Assert(maxCapacity >= capacity);

    _list = new List<T>(capacity);

    Capacity = maxCapacity;
  }

  public int Count => _list.Count;
  public int Capacity { get; }

  public bool Contains(T value)
  {
    return _list.Contains(value);
  }

  public bool TryAdd(T value)
  {
    if (_list.Count < Capacity)
    {
      _list.Add(value);
      return true;
    }

    return false;
  }

  public void Remove(T value)
  {
    _list.Remove(value);
  }

  public void Clear()
  {
    _list.Clear();
  }

  public List<T>.Enumerator GetEnumerator()
  {
    return _list.GetEnumerator();
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
