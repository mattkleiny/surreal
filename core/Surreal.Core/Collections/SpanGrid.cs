using System;
using System.Diagnostics;
using Surreal.Mathematics.Grids;

namespace Surreal.Collections {
  [DebuggerDisplay("SpanGrid {Width}x{Height}")]
  public ref struct SpanGrid<T> {
    public static SpanGrid<T> Empty => default;

    private readonly Span<T> span;

    public SpanGrid(Span<T> span, int width, int height) {
      this.span = span;

      Width  = width;
      Height = height;
    }

    public SpanGrid(Span<T> span, int stride) {
      this.span = span;

      Width  = span.Length % stride;
      Height = span.Length / stride;
    }

    public int     Length => span.Length;
    public int     Width  { get; }
    public int     Height { get; }
    public Span<T> Span   => span;

    public ref T this[Index index] => ref span[index];
    public ref T this[int x, int y] => ref span[x + y * Width];

    public GridEnumerator EnumerateCells() {
      return new(Width, Height);
    }

    public static implicit operator ReadOnlySpan<T>(SpanGrid<T> grid) => grid.Span;
  }
}