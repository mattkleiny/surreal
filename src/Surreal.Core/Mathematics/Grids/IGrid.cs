using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Surreal.Collections;
using Surreal.Mathematics.Curves;
using Surreal.Mathematics.Linear;

namespace Surreal.Mathematics.Grids {
  public enum GridSamplingMode {
    Assert,
    Clamp,
    Wrap
  }

  public interface IGrid<T> {
    int Width  { get; }
    int Height { get; }

    T this[int x, int y] { get; set; }

    T Sample(int x, int y, GridSamplingMode mode = GridSamplingMode.Assert) {
      switch (mode) {
        case GridSamplingMode.Assert: {
          Debug.Assert(IsValid(x, y), "Contains(x,y)");
          break;
        }
        case GridSamplingMode.Clamp: {
          if (x < 0) x          = 0;
          if (y < 0) y          = 0;
          if (x > Width - 1) x  = Width - 1;
          if (y > Height - 1) y = Height - 1;
          break;
        }
        case GridSamplingMode.Wrap: {
          if (x < 0) x          = Width - 1;
          if (y < 0) y          = Height - 1;
          if (x > Width - 1) x  = 0;
          if (y > Height - 1) y = 0;
          break;
        }
      }

      return this[x, y];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IsValid(int x, int y) {
      return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    T TryGet(int x, int y, T defaultValue = default) {
      return IsValid(x, y) ? this[x, y] : defaultValue;
    }

    void Fill(T value) {
      for (var y = 0; y < Height; y++)
      for (var x = 0; x < Width; x++) {
        this[x, y] = value;
      }
    }
  }

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

      if (System.MathF.Abs(direction.X) > System.MathF.Abs(direction.Y)) {
        stepCount = (int) System.MathF.Ceiling(System.MathF.Abs(direction.X));
        step      = new Vector2(1, direction.Y / direction.X) * System.MathF.Sign(direction.X);
      } else {
        stepCount = (int) System.MathF.Ceiling(System.MathF.Abs(direction.Y));
        step      = new Vector2(direction.X / direction.Y, 1) * System.MathF.Sign(direction.Y);
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

      for (var i = 1; i < points.Length; i++) {
        target.DrawLine(points[i - 1], points[i], value);
      }
    }
  }
}