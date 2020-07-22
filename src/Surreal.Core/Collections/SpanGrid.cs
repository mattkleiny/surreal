using System;
using System.Diagnostics;
using Surreal.Mathematics.Grids;

namespace Surreal.Collections {
  [DebuggerDisplay("SpanGrid {Width}x{Height}")]
  public ref struct SpanGrid<T>
      where T : unmanaged {
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

    public T this[Index index] {
      get => span[index];
      set => span[index] = value;
    }

    public T this[int x, int y] {
      get => span[x + y * Width];
      set => span[x + y * Width] = value;
    }

    public GridEnumerator EnumerateCells() {
      return new GridEnumerator(Width, Height);
    }

    public static implicit operator ReadOnlySpan<T>(SpanGrid<T> grid) => grid.Span;
  }
}