using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Surreal.Collections.Spans {
  [DebuggerDisplay("GridSpan {Length} elements ({Width}x{Height})")]
  public readonly ref struct SpanGrid<T>
      where T : unmanaged {
    public static SpanGrid<T> Empty => default;

    private readonly Span<T> span;
    private readonly int     stride;

    public SpanGrid(Span<T> span, int stride) {
      this.span   = span;
      this.stride = stride;
    }

    public int Width  => stride;
    public int Height => span.Length / stride;
    public int Length => span.Length;

    public ref T this[int index] {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get {
        Debug.Assert(index >= 0, "index >= 0");
        Debug.Assert(index < span.Length, "index < length");

        return ref span[index];
      }
    }

    public ref T this[int x, int y] {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get {
        Debug.Assert(x >= 0 && x < Width, "x >= 0 && x < Width");
        Debug.Assert(y >= 0 && y < Height, "y >= 0 && y < Height");

        return ref span[x + y * stride];
      }
    }
  }
}