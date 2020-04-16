using System;
using System.Diagnostics;

namespace Surreal.Collections
{
  [DebuggerDisplay("SpanGrid {Width}x{Height}")]
  public ref struct SpanGrid<T>
    where T : unmanaged
  {
    public static SpanGrid<T> Empty => default;

    private readonly Span<T> span;

    public SpanGrid(Span<T> span, int width, int height)
    {
      this.span = span;

      Width  = width;
      Height = height;
    }

    public SpanGrid(Span<T> span, int stride)
    {
      this.span = span;

      Width  = span.Length % stride;
      Height = span.Length / stride;
    }

    public int Capacity => span.Length;
    public int Width    { get; }
    public int Height   { get; }

    public T this[int x, int y]
    {
      get => span[x + y * Width];
      set => span[x + y * Width] = value;
    }

    public ReadOnlySpan<T> ToSpan()  => span;
    public T[]             ToArray() => ToSpan().ToArray();
  }
}