using System;
using System.Diagnostics;
using System.Numerics;
using Surreal.Collections.Spans;
using Surreal.Mathematics.Linear;

namespace Surreal.Mathematics.Grids {
  public static class GridExtensions {
    public static void DrawRect<T>(this IGrid<T> grid, Rectangle rectangle, T value) {
      var (left, top, right, bottom) = rectangle;

      for (var y = (int) top; y < (int) bottom; y++)
      for (var x = (int) left; x < (int) right; x++) {
        grid[x, y] = value;
      }
    }

    public static void DrawLine<T>(this IGrid<T> grid, Vector2 from, Vector2 to, T value) {
      var direction = to - from;

      int     stepCount;
      Vector2 step;

      if (MathF.Abs(direction.X) > MathF.Abs(direction.Y)) {
        stepCount = (int) MathF.Ceiling(MathF.Abs(direction.X));
        step      = new Vector2(1, direction.Y / direction.X) * MathF.Sign(direction.X);
      }
      else {
        stepCount = (int) MathF.Ceiling(MathF.Abs(direction.Y));
        step      = new Vector2(direction.X / direction.Y, 1) * MathF.Sign(direction.Y);
      }

      var point = from;

      for (var i = 0; i < stepCount; i++) {
        var x = (int) point.X;
        var y = (int) point.Y;

        grid[x, y] = value;

        point += step;
      }
    }

    public static void DrawCurve<T, TCurve>(this IGrid<T> target, in TCurve curve, T value, int resolution = 32)
        where TCurve : struct, IPlanarCurve {
      Debug.Assert(resolution > 2, "resolution > 2");

      var points = new SpanList<Vector2>(stackalloc Vector2[resolution]);

      for (var i = 0; i < resolution; i++) {
        points.Add(curve.SampleAt(i / (float) points.Capacity));
      }

      for (var i = 1; i < points.Count; i++) {
        target.DrawLine(points[i - 1], points[i], value);
      }
    }
  }
}