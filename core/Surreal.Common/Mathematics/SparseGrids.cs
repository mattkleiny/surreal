namespace Surreal.Mathematics;

/// <summary>A 2d grid of <see cref="T"/>.</summary>
public sealed class SparseGrid<T> : IEnumerable<T>
  where T : class
{
  private readonly Dictionary<Point2, T> items = new();

  public T? this[Point2 position]
  {
    get
    {
      if (!items.TryGetValue(position, out var item))
      {
        return null;
      }

      return item;
    }
    set
    {
      if (value != null)
      {
        items[position] = value;
      }
      else
      {
        items.Remove(position);
      }
    }
  }

  public Dictionary<Point2, T>.ValueCollection.Enumerator GetEnumerator()
  {
    return items.Values.GetEnumerator();
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return items.Values.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
