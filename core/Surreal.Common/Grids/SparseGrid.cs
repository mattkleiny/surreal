using Surreal.Mathematics;

namespace Surreal.Grids;

/// <summary>
/// A sparse 2d grid of <see cref="T" />.
/// </summary>
public sealed class SparseGrid<T> : IEnumerable<T>
{
  private readonly Dictionary<Point2, T> _items = new();

  public T? this[int x, int y]
  {
    get => this[new Point2(x, y)];
    set => this[new Point2(x, y)] = value;
  }

  public T? this[Point2 position]
  {
    get
    {
      if (!_items.TryGetValue(position, out var item))
      {
        return default;
      }

      return item;
    }
    set
    {
      if (value != null)
      {
        _items[position] = value;
      }
      else
      {
        _items.Remove(position);
      }
    }
  }

  public void Clear()
  {
    _items.Clear();
  }

  public Dictionary<Point2, T>.ValueCollection.Enumerator GetEnumerator()
  {
    return _items.Values.GetEnumerator();
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return _items.Values.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
