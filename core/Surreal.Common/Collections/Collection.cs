namespace Surreal.Collections;

/// <summary>
/// A generic collection of objects that can be individually accessed by index.
/// <para/>
/// This type is a wrapper around <see cref="List{T}"/> that forms a base class for custom collections.
/// </summary>
public class Collection<T> : IList<T>
{
  public delegate void ItemChangeHandler(Collection<T> collection, T item);

  private readonly List<T> _items;

  public Collection()
  {
    _items = [];
  }

  public Collection(int capacity)
  {
    _items = new List<T>(capacity);
  }

  public event ItemChangeHandler? Added;
  public event ItemChangeHandler? Removed;

  public int Count => _items.Count;
  public bool IsReadOnly => false;

  public T this[int index]
  {
    get => _items[index];
    set => _items[index] = value;
  }

  public T this[Index index]
  {
    get => _items[index];
    set => _items[index] = value;
  }

  public Slice<T> this[Range range]
  {
    get
    {
      var (offset, length) = range.GetOffsetAndLength(Count);

      return _items.AsSlice(offset, length);
    }
  }

  protected virtual void OnItemAdded(T item)
  {
    Added?.Invoke(this, item);
  }

  protected virtual void OnItemRemoved(T item)
  {
    Removed?.Invoke(this, item);
  }

  public bool Contains(T item)
  {
    return _items.Contains(item);
  }

  public void Add(T item)
  {
    _items.Add(item);
    OnItemAdded(item);
  }

  public bool Remove(T item)
  {
    if (_items.Remove(item))
    {
      OnItemRemoved(item);
      return true;
    }

    return false;
  }

  public void Clear()
  {
    for (var i = _items.Count - 1; i >= 0; i--)
    {
      var item = _items[i];

      _items.RemoveAt(i);
      OnItemRemoved(item);
    }
  }

  public int IndexOf(T item)
  {
    return _items.IndexOf(item);
  }

  public void CopyTo(T[] array, int arrayIndex)
  {
    _items.CopyTo(array, arrayIndex);
  }

  public void Insert(int index, T item)
  {
    _items.Insert(index, item);
    OnItemAdded(item);
  }

  public void RemoveAt(int index)
  {
    var item = _items[index];

    _items.RemoveAt(index);
    OnItemRemoved(item);
  }

  public List<T>.Enumerator GetEnumerator()
  {
    return _items.GetEnumerator();
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
