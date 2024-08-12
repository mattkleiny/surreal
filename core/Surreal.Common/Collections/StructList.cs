namespace Surreal.Collections;

/// <summary>
/// A list of structs with optimized memory layout and access patterns.
/// </summary>
public sealed class StructList<T>(int capacity = 16) : IEnumerable<T>
  where T : struct
{
  private T[] _elements = new T[capacity];
  private int _count;

  /// <summary>
  /// The number of elements in the list.
  /// </summary>
  public int Count => _count;

  /// <summary>
  /// The total number of elements that can fit in the list (at the current size).
  /// </summary>
  public int Capacity => _elements.Length;

  /// <summary>
  /// Accesses an element in the list by index.
  /// </summary>
  public ref T this[int index] => ref _elements[index];

  /// <summary>
  /// Accesses an element in the list by index.
  /// </summary>
  public ref T this[Index index] => ref _elements[index];

  /// <summary>
  /// Determines whether an element is in the list.
  /// </summary>
  public bool Contains(T value)
  {
    for (var i = 0; i < _count; i++)
    {
      if (_elements[i].Equals(value))
      {
        return true;
      }
    }

    return false;
  }

  /// <summary>
  /// Adds an element to the end of the list.
  /// </summary>
  public void Add(T value)
  {
    if (_count + 1 > _elements.Length)
    {
      Array.Resize(ref _elements, _elements.Length * 2);
    }

    _elements[_count++] = value;
  }

  /// <summary>
  /// Removes the first occurrence of a specific object from the list.
  /// </summary>
  public void Remove(T value)
  {
    for (var i = 0; i < _count; i++)
    {
      if (_elements[i].Equals(value))
      {
        _elements[i] = _elements[_count - 1];
        _count--;

        break;
      }
    }
  }

  /// <summary>
  /// Clears the list of all elements.
  /// </summary>
  public void Clear()
  {
    Array.Clear(_elements);
    
    _count = 0;
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
  /// Enumerates the elements of a <see cref="StructList{T}"/>.
  /// </summary>
  public struct Enumerator : IEnumerator<T>
  {
    private readonly StructList<T> _list;
    private int _index;

    public Enumerator(StructList<T> list)
    {
      _list = list;
      Reset();
    }

    public ref T Current => ref _list[_index];
    T IEnumerator<T>.Current => _list[_index];
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      return ++_index < _list.Count;
    }

    public void Reset()
    {
      _index = -1;
    }

    public void Dispose()
    {
      // no-op
    }
  }
}
