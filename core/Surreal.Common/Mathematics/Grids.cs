using Surreal.Memory;

namespace Surreal.Mathematics;

/// <summary>
/// A dense 2d grid of <see cref="T" />.
/// </summary>
public sealed class Grid<T> : IEnumerable<T>
{
  private readonly T[] _elements;

  public Grid(int width, int height)
  {
    Debug.Assert(width > 0, "width > 0");
    Debug.Assert(height > 0, "height > 0");

    _elements = new T[width * height];

    Width = width;
    Height = height;
  }

  public Grid(int width, int height, T defaultValue)
    : this(width, height)
  {
    Fill(defaultValue);
  }

  public int Width { get; }
  public int Height { get; }

  public SpanGrid<T> Span => new(_elements, Width);

  public ref T this[Point2 position] => ref this[position.X, position.Y];

  public ref T this[int x, int y]
  {
    get
    {
      Debug.Assert(x >= 0 && x < Width, "x >= 0 && x < Width");
      Debug.Assert(y >= 0 && y < Height, "y >= 0 && y < Height");

      return ref _elements[x + y * Width];
    }
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  public void Fill(T value)
  {
    for (var i = 0; i < _elements.Length; i++) _elements[i] = value;
  }

  public Enumerator GetEnumerator()
  {
    return new Enumerator(this);
  }

  /// <summary>
  /// Custom enumerator for <see cref="Grid{T}" />s.
  /// </summary>
  public struct Enumerator : IEnumerator<T>
  {
    private readonly Grid<T> _grid;
    private int _index;

    public Enumerator(Grid<T> grid)
      : this()
    {
      _grid = grid;
      Reset();
    }

    public T Current => _grid._elements[_index]!;
    object IEnumerator.Current => Current!;

    public bool MoveNext()
    {
      return ++_index < _grid._elements.Length;
    }

    public void Reset()
    {
      _index = -1;
    }

    public void Dispose()
    {
    }
  }
}

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

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return _items.Values.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  public void Clear()
  {
    _items.Clear();
  }

  public Dictionary<Point2, T>.ValueCollection.Enumerator GetEnumerator()
  {
    return _items.Values.GetEnumerator();
  }
}
