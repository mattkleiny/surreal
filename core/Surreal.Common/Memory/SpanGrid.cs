namespace Surreal.Memory;

/// <summary>A <see cref="Span{T}"/> that is interpreted as a grid.</summary>
[DebuggerDisplay("SpanGrid {Length} elements ({Width}x{Height})")]
public readonly ref struct SpanGrid<T>
  where T : unmanaged
{
  public static SpanGrid<T> Empty => default;

  private readonly Span<T> storage;
  private readonly int     stride;

  public SpanGrid(Span<T> storage, int stride)
  {
    this.storage = storage;
    this.stride  = stride;
  }

  public int Width  => stride;
  public int Height => storage.Length / stride;
  public int Length => storage.Length;

  public ref T this[int x, int y]
  {
    get
    {
      Debug.Assert(x >= 0 && x < Width, "x >= 0 && x < Width");
      Debug.Assert(y >= 0 && y < Height, "y >= 0 && y < Height");

      return ref storage[x + y * stride];
    }
  }

  public Span<T> ToSpan()
  {
    return storage;
  }

  public static implicit operator Span<T>(SpanGrid<T> grid) => grid.ToSpan();
}
