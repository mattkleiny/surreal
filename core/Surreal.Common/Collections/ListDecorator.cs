namespace Surreal.Collections;

/// <summary>Base class for any custom collection of <see cref="T"/> that supports decorating default behaviour.</summary>
public abstract class ListDecorator<T> : ICollection<T>
{
  protected List<T> Items { get; } = new();

  public int  Count      => Items.Count;
  public bool IsReadOnly => false;

  public virtual bool Contains(T item)
  {
    return Items.Contains(item);
  }

  public virtual void Add(T item)
  {
    Items.Add(item);

    OnItemAdded(item);
  }

  public virtual bool Remove(T item)
  {
    if (Items.Remove(item))
    {
      OnItemRemoved(item);

      return true;
    }

    return false;
  }

  public virtual void CopyTo(T[] array, int arrayIndex)
  {
    Items.CopyTo(array, arrayIndex);
  }

  public virtual void Clear()
  {
    for (var i = Items.Count - 1; i >= 0; i--)
    {
      OnItemRemoved(Items[i]);

      Items.RemoveAt(i);
    }
  }

  protected virtual void OnItemAdded(T item)
  {
  }

  protected virtual void OnItemRemoved(T item)
  {
  }

  public List<T>.Enumerator GetEnumerator()
  {
    return Items.GetEnumerator();
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
