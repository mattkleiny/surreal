using System.Runtime.CompilerServices;
using Surreal.Mathematics;

namespace Surreal.Memory;

/// <summary>A <see cref="Span{T}"/> that is interpreted as a grid.</summary>
[DebuggerDisplay("SpanGrid {Length} elements ({Width}x{Height})")]
public readonly ref struct SpanGrid<T>
{
  public static SpanGrid<T> Empty => default;

  private readonly Span<T> storage;
  private readonly int stride;

  public SpanGrid(Span<T> storage, int stride)
  {
    this.storage = storage;
    this.stride  = stride;

    Height = stride > 0 ? storage.Length / stride : 0;
  }

  public int Width  => stride;
  public int Height { get; }
  public int Length => storage.Length;

  public ref T this[Point2 position] => ref this[position.X, position.Y];

  public ref T this[int x, int y]
  {
    get
    {
      Debug.Assert(x >= 0 && x < Width, "x >= 0 && x < Width");
      Debug.Assert(y >= 0 && y < Height, "y >= 0 && y < Height");

      return ref storage[x + y * stride];
    }
  }

  public ref T this[Index x, Index y]
  {
    get
    {
      var ix = x.GetOffset(Width);
      var iy = y.GetOffset(Height);

      return ref storage[ix + iy * stride];
    }
  }

  public void Fill(T value)
  {
    storage.Fill(value);
  }

  public Span<T> ToSpan()
  {
    return storage;
  }

  public ReadOnlySpan<T> ToReadOnlySpan()
  {
    return storage;
  }

  public static implicit operator Span<T>(SpanGrid<T> grid) => grid.ToSpan();
  public static implicit operator ReadOnlySpan<T>(SpanGrid<T> grid) => grid.ToSpan();
  public static implicit operator ReadOnlySpanGrid<T>(SpanGrid<T> grid) => new(grid.storage, grid.stride);
}

/// <summary>A <see cref="ReadOnlySpan{T}"/> that is interpreted as a grid.</summary>
[DebuggerDisplay("ReadOnlySpanGrid {Length} elements ({Width}x{Height})")]
public readonly ref struct ReadOnlySpanGrid<T>
{
  public static ReadOnlySpanGrid<T> Empty => default;

  private readonly ReadOnlySpan<T> storage;
  private readonly int stride;

  public ReadOnlySpanGrid(ReadOnlySpan<T> storage, int stride)
  {
    this.storage = storage;
    this.stride  = stride;

    Height = stride > 0 ? storage.Length / stride : 0;
  }

  public int Width  => stride;
  public int Height { get; }
  public int Length => storage.Length;

  public T this[Point2 position] => this[position.X, position.Y];

  public T this[int x, int y]
  {
    get
    {
      Debug.Assert(x >= 0 && x < Width, "x >= 0 && x < Width");
      Debug.Assert(y >= 0 && y < Height, "y >= 0 && y < Height");

      return storage[x + y * stride];
    }
  }

  public T this[Index x, Index y]
  {
    get
    {
      var ix = x.GetOffset(Width);
      var iy = y.GetOffset(Height);

      return storage[ix + iy * stride];
    }
  }

  public ReadOnlySpan<T> ToReadOnlySpan()
  {
    return storage;
  }

  public static implicit operator ReadOnlySpan<T>(ReadOnlySpanGrid<T> grid) => grid.ToReadOnlySpan();
}

/// <summary>Static extensions for dealing with <see cref="SpanGrid{T}"/>s.</summary>
public static class SpanGridExtensions
{
  public delegate TOut Painter<in TIn, out TOut>(int x, int y, TIn value);

  /// <summary>Converts a <see cref="Span{T}"/> to a <see cref="SpanGrid{T}"/> with the given stride between rows.</summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static SpanGrid<T> ToGrid<T>(this Span<T> span, int stride) => new(span, stride);

  /// <summary>Converts a <see cref="ReadOnlySpan{T}"/> to a <see cref="ReadOnlySpanGrid{T}"/> with the given stride between rows.</summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ReadOnlySpanGrid<T> ToReadOnlyGrid<T>(this ReadOnlySpan<T> span, int stride) => new(span, stride);

  /// <summary>Draws a circle in the grid.</summary>
  public static void DrawCircle<T>(this SpanGrid<T> grid, Point2 center, int radius, T value)
  {
    var rectangle = Rectangle.Create(center, new Point2(radius, radius));
    var clamped = rectangle.Clamp(0, 0, grid.Width - 1, grid.Height - 1);

    foreach (var point in clamped.Points)
    {
      var distance = point - center;
      if (distance.LengthSquared() < radius)
      {
        grid[point] = value;
      }
    }
  }

  /// <summary>Draws a rectangle in the grid.</summary>
  public static void DrawRectangle<T>(this SpanGrid<T> grid, Point2 center, Point2 size, T value)
  {
    var rectangle = Rectangle.Create(center, size);
    var clamped = rectangle.Clamp(0, 0, grid.Width - 1, grid.Height - 1);

    foreach (var point in clamped.Points)
    {
      grid[point] = value;
    }
  }

  /// <summary>Draws a rectangle in the grid.</summary>
  public static void DrawRectangle<T>(this SpanGrid<T> grid, Rectangle rectangle, T value)
  {
    foreach (var point in rectangle.Points)
    {
      grid[point] = value;
    }
  }

  /// <summary>Draws a line in the grid.</summary>
  public static void DrawLine<T>(this SpanGrid<T> grid, Point2 from, Point2 to, T value)
  {
    // bresenham line algorithm
    var (x0, x1) = (from.X, to.X);
    var (y0, y1) = (from.Y, to.Y);

    int dx = Math.Abs(x1 - x0);
    int sx = x0 < x1 ? 1 : -1;

    int dy = Math.Abs(y1 - y0);
    int sy = y0 < y1 ? 1 : -1;

    int error = (dx > dy ? dx : -dy) / 2;

    while (!(x0 == x1 && y0 == y1))
    {
      grid[x0, y0] = value;

      var e2 = error;

      if (e2 > -dx)
      {
        error -= dy;
        x0    += sx;
      }

      if (e2 < dy)
      {
        error += dx;
        y0    += sy;
      }
    }
  }

  /// <summary>Draws a strip of lines in the grid.</summary>
  public static void DrawLineStrip<T>(this SpanGrid<T> grid, ReadOnlySpan<Point2> positions, T value)
  {
    for (var i = 1; i < positions.Length; i++)
    {
      var from = positions[i - 1];
      var to = positions[i];

      grid.DrawLine(from, to, value);
    }
  }

  /// <summary>Draws a curve in the grid.</summary>
  public static void DrawCurve<T, TCurve>(this SpanGrid<T> grid, TCurve curve, T value, int samples = 16)
    where TCurve : IPlanarCurve
  {
    // collect all points
    var positions = new SpanList<Point2>(stackalloc Point2[samples]);
    var delta = 1f / samples;

    for (int i = 0; i < samples; i++)
    {
      positions.Add(curve.SampleAt(delta * i));
    }

    // draw line strip
    grid.DrawLineStrip(positions.ToSpan(), value);
  }

  /// <summary>Paints this grid onto another via a given <see cref="painter"/> function.</summary>
  public static void PaintTo<TIn, TOut>(this SpanGrid<TIn> from, SpanGrid<TOut> to, Painter<TIn, TOut> painter)
  {
    for (var y = 0; y < from.Height; y++)
    for (var x = 0; x < from.Width; x++)
    {
      to[x, y] = painter(x, y, from[x, y]);
    }
  }

  /// <summary>Converts the grid to a string, using the given <see cref="painter"/> function.</summary>
  public static string ToString<T>(this SpanGrid<T> grid, Painter<T?, char> painter)
  {
    var builder = new StringBuilder();

    for (var y = 0; y < grid.Height; y++)
    {
      if (y > 0) builder.AppendLine();

      for (var x = 0; x < grid.Width; x++)
      {
        var tile = grid[x, y];
        var rune = painter(x, y, tile);

        builder.Append(rune);
      }
    }

    return builder.ToString();
  }
}
