namespace Surreal.Mathematics;

/// <summary>A 2d grid of <see cref="T"/>.</summary>
public sealed class SparseGrid<T> : IEnumerable<T>
{
  private readonly Dictionary<Point2, T> items = new();

  public T? this[int x, int y]
  {
    get => this[new Point2(x, y)];
    set => this[new Point2(x, y)] = value;
  }

  public T? this[Point2 position]
  {
    get
    {
      if (!items.TryGetValue(position, out var item))
      {
        return default;
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
