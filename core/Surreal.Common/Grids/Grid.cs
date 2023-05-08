using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Grids;

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

  public void Fill(T value)
  {
    for (var i = 0; i < _elements.Length; i++) _elements[i] = value;
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
