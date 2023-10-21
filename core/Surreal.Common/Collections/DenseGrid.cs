using Surreal.Maths;
using Surreal.Memory;

namespace Surreal.Collections;

/// <summary>
/// A dense 2d grid of <see cref="T" />.
/// </summary>
public sealed class DenseGrid<T> : IEnumerable<T>
{
  private readonly T[] _elements;

  public DenseGrid(int width, int height)
  {
    Debug.Assert(width > 0, "width > 0");
    Debug.Assert(height > 0, "height > 0");

    _elements = new T[width * height];

    Width = width;
    Height = height;
  }

  public DenseGrid(int width, int height, T defaultValue)
    : this(width, height)
  {
    Fill(defaultValue);
  }

  public int Width { get; }
  public int Height { get; }

  /// <summary>
  /// Converts the grid to a <see cref="SpanGrid{T}" />.
  /// </summary>
  public SpanGrid<T> Span => new(_elements, Width);

  /// <summary>
  /// Returns a reference to the element at the given position.
  /// </summary>
  public ref T this[int x, int y]
  {
    get
    {
      Debug.Assert(x >= 0 && x < Width, "x >= 0 && x < Width");
      Debug.Assert(y >= 0 && y < Height, "y >= 0 && y < Height");

      return ref _elements[x + y * Width];
    }
  }

  /// <summary>
  /// Accesses the element at the given position.
  /// </summary>
  public ref T this[Point2 position] => ref this[position.X, position.Y];

  /// <summary>
  /// Accesses the element at the given position.
  /// </summary>
  public ref T this[(int X, int Y) position] => ref this[position.X, position.Y];

  /// <summary>
  /// Fills the grid with the given value.
  /// </summary>
  public void Fill(T value)
  {
    Array.Fill(_elements, value);
  }

  /// <summary>
  /// Returns a <see cref="Span{T}" /> of the grid's elements.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Span<T> AsSpan()
  {
    return _elements;
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
  /// Custom enumerator for <see cref="DenseGrid{T}"/>s.
  /// </summary>
  public struct Enumerator : IEnumerator<T>
  {
    private readonly DenseGrid<T> _grid;
    private int _index;

    public Enumerator(DenseGrid<T> grid)
      : this()
    {
      _grid = grid;
      Reset();
    }

    public ref T Current => ref _grid._elements[_index]!;
    T IEnumerator<T>.Current => _grid._elements[_index]!;
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
