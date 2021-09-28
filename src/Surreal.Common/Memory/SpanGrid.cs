using System;
using System.Diagnostics;

namespace Surreal.Memory
{
  /// <summary>A <see cref="Span{T}"/> that is interpreted as a grid.</summary>
  [DebuggerDisplay("SpanGrid {Length} elements ({Width}x{Height})")]
  public readonly ref struct SpanGrid<T>
      where T : unmanaged
  {
    public static SpanGrid<T> Empty => default;

    private readonly Span<T> span;
    private readonly int     stride;

    public SpanGrid(Span<T> span, int stride)
    {
      this.span   = span;
      this.stride = stride;
    }

    public int Width  => stride;
    public int Height => span.Length / stride;
    public int Length => span.Length;

    public ref T this[int index]
    {
      get
      {
        Debug.Assert(index >= 0, "index >= 0");
        Debug.Assert(index < span.Length, "index < length");

        return ref span[index];
      }
    }

    public ref T this[int x, int y]
    {
      get
      {
        Debug.Assert(x >= 0 && x < Width, "x >= 0 && x < Width");
        Debug.Assert(y >= 0 && y < Height, "y >= 0 && y < Height");

        return ref span[x + y * stride];
      }
    }
  }
}
