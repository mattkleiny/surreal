using Surreal.Mathematics;

namespace Surreal.Memory;

/// <summary>
/// A <see cref="Span{T}" /> that is interpreted as a grid.
/// </summary>
[DebuggerDisplay("SpanGrid {Length} elements ({Width}x{Height})")]
public readonly ref struct SpanGrid<T>(Span<T> storage, int stride)
{
  public static SpanGrid<T> Empty => default;

  private readonly Span<T> _storage = storage;

  public int Width { get; } = stride;

  public int Height { get; } = stride > 0 ? storage.Length / stride : 0;
  public int Length => _storage.Length;

  public ref T this[Point2 position] => ref this[position.X, position.Y];

  public ref T this[int x, int y]
  {
    get
    {
      Debug.Assert(x >= 0 && x < Width, "x >= 0 && x < Width");
      Debug.Assert(y >= 0 && y < Height, "y >= 0 && y < Height");

      return ref _storage[x + y * Width];
    }
  }

  public ref T this[Index x, Index y]
  {
    get
    {
      var ix = x.GetOffset(Width);
      var iy = y.GetOffset(Height);

      return ref _storage[ix + iy * Width];
    }
  }

  public void Fill(T value)
  {
    _storage.Fill(value);
  }

  public Span<T> ToSpan()
  {
    return _storage;
  }

  public ReadOnlySpan<T> ToReadOnlySpan()
  {
    return _storage;
  }

  public static implicit operator Span<T>(SpanGrid<T> grid)
  {
    return grid.ToSpan();
  }

  public static implicit operator ReadOnlySpan<T>(SpanGrid<T> grid)
  {
    return grid.ToSpan();
  }

  public static implicit operator ReadOnlySpanGrid<T>(SpanGrid<T> grid)
  {
    return new ReadOnlySpanGrid<T>(grid._storage, grid.Width);
  }
}

/// <summary>
/// A <see cref="ReadOnlySpan{T}" /> that is interpreted as a grid.
/// </summary>
[DebuggerDisplay("ReadOnlySpanGrid {Length} elements ({Width}x{Height})")]
public readonly ref struct ReadOnlySpanGrid<T>(ReadOnlySpan<T> storage, int stride)
{
  public static ReadOnlySpanGrid<T> Empty => default;

  private readonly ReadOnlySpan<T> _storage = storage;

  public int Width { get; } = stride;

  public int Height { get; } = stride > 0 ? storage.Length / stride : 0;
  public int Length => _storage.Length;

  public T this[Point2 position] => this[position.X, position.Y];

  public ref readonly T this[int x, int y]
  {
    get
    {
      Debug.Assert(x >= 0 && x < Width, "x >= 0 && x < Width");
      Debug.Assert(y >= 0 && y < Height, "y >= 0 && y < Height");

      return ref _storage[x + y * Width];
    }
  }

  public ref readonly T this[Index x, Index y]
  {
    get
    {
      var ix = x.GetOffset(Width);
      var iy = y.GetOffset(Height);

      return ref _storage[ix + iy * Width];
    }
  }

  public ReadOnlySpan<T> ToReadOnlySpan()
  {
    return _storage;
  }

  public static implicit operator ReadOnlySpan<T>(ReadOnlySpanGrid<T> grid)
  {
    return grid.ToReadOnlySpan();
  }
}
