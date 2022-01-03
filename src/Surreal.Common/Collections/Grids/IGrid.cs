using System.Diagnostics;
using System.Numerics;
using Surreal.Mathematics;
using Surreal.Memory;

namespace Surreal.Collections.Grids;

/// <summary>A grid of elements in 2-space.</summary>
public interface IGrid<T>
{
  public int Width  { get; }
  public int Height { get; }

  public T? this[int x, int y] { get; set; }

  bool IsValid(int x, int y)
  {
    return x >= 0 && x < Width && y >= 0 && y < Height;
  }

  void Fill(T value)
  {
    for (var y = 0; y < Height; y++)
    for (var x = 0; x < Width; x++)
    {
      this[x, y] = value;
    }
  }
}

/// <summary>A <see cref="IGrid{T}"/> with direct field access to <see cref="T"/>.</summary>
public interface IDirectAccessGrid<T> : IGrid<T>
{
  new ref T? this[int x, int y] { get; }
}

/// <summary>Extensions for working with <see cref="IGrid{T}"/>s.</summary>
public static class GridExtensions
{
  public static void DrawLine<T>(this IGrid<T> grid, Vector2 from, Vector2 to, T value)
  {
    var direction = to - from;

    int     stepCount;
    Vector2 step;

    if (MathF.Abs(direction.X) > MathF.Abs(direction.Y))
    {
      stepCount = (int) MathF.Ceiling(MathF.Abs(direction.X));
      step      = new Vector2(1, direction.Y / direction.X) * MathF.Sign(direction.X);
    }
    else
    {
      stepCount = (int) MathF.Ceiling(MathF.Abs(direction.Y));
      step      = new Vector2(direction.X / direction.Y, 1) * MathF.Sign(direction.Y);
    }

    var point = from;

    for (var i = 0; i < stepCount; i++)
    {
      var x = (int) point.X;
      var y = (int) point.Y;

      grid[x, y] = value;

      point += step;
    }
  }

  public static void DrawRectangle<T>(this IGrid<T> grid, Rectangle rectangle, T value)
  {
    var (left, top, right, bottom) = rectangle;

    for (var y = (int) top; y < (int) bottom; y++)
    for (var x = (int) left; x < (int) right; x++)
    {
      grid[x, y] = value;
    }
  }

  public static void DrawCurve<T, TCurve>(this IGrid<T> grid, in TCurve curve, T value, int resolution = 32)
    where TCurve : struct, IPlanarCurve
  {
    Debug.Assert(resolution > 2, "resolution > 2");

    var points = new SpanList<Vector2>(stackalloc Vector2[resolution]);

    for (var i = 0; i < resolution; i++)
    {
      points.Add(curve.SampleAt(i / (float) points.Capacity));
    }

    for (var i = 1; i < points.Count; i++)
    {
      grid.DrawLine(points[i - 1], points[i], value);
    }
  }
}
