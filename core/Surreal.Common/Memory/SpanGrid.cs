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

    Height = storage.Length / stride;
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
}

/// <summary>Static extensions for dealing with <see cref="SpanGrid{T}"/>s.</summary>
public static class SpanGridExtensions
{
  public delegate TOut Painter<in TIn, out TOut>(int x, int y, TIn value);

  /// <summary>Converts a <see cref="Span{T}"/> to a <see cref="SpanGrid{T}"/> with the given stride between rows.</summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static SpanGrid<T> ToGrid<T>(this Span<T> span, int stride) => new(span, stride);

  /// <summary>Draws a circle in the grid.</summary>
  public static void DrawCircle<T>(this SpanGrid<T> grid, Point2 center, int radius, T value)
  {
    var rectangle = Rectangle
      .Create(center, new Point2(radius, radius))
      .Clamp(0, 0, grid.Width - 1, grid.Height - 1);

    for (int y = rectangle.Bottom; y < rectangle.Top; y++)
    for (int x = rectangle.Left; x < rectangle.Right; x++)
    {
      var point = new Point2(x, y);
      var distance = point - center;

      if (distance.LengthSquared() < radius)
      {
        grid[x, y] = value;
      }
    }
  }

  /// <summary>Draws a rectangle in the grid.</summary>
  public static void DrawRectangle<T>(this SpanGrid<T> grid, Point2 center, Point2 size, T value)
  {
    var rectangle = Rectangle
      .Create(center, size)
      .Clamp(0, 0, grid.Width - 1, grid.Height - 1);

    for (int y = rectangle.Bottom; y < rectangle.Top; y++)
    for (int x = rectangle.Left; x < rectangle.Right; x++)
    {
      var point = new Point2(x, y);

      if (rectangle.Contains(point))
      {
        grid[x, y] = value;
      }
    }
  }

  /// <summary>Draws a line in the grid.</summary>
  public static void DrawLine<T>(this SpanGrid<T> grid, Point2 from, Point2 to, T value)
  {
    throw new NotImplementedException();
  }

  /// <summary>Draws a curve in the grid.</summary>
  public static void DrawCurve<T, TCurve>(this SpanGrid<T> grid, TCurve curve, T value)
    where TCurve : IPlanarCurve
  {
    throw new NotImplementedException();
  }

  /// <summary>Blits one grid to another.</summary>
  public static void PaintTo<TIn, TOut>(this SpanGrid<TIn> from, SpanGrid<TOut> to, Painter<TIn, TOut> painter)
  {
    for (var y = 0; y < from.Height; y++)
    for (var x = 0; x < from.Width; x++)
    {
      to[x, y] = painter(x, y, from[x, y]);
    }
  }

  /// <summary>Converts a grid to a string, using the given painting function.</summary>
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
