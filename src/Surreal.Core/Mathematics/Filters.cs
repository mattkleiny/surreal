using System;
using System.Drawing;
using System.Numerics;
using Surreal.Mathematics.Grids;
using Surreal.Mathematics.Linear;
using Rectangle = Surreal.Mathematics.Linear.Rectangle;

namespace Surreal.Mathematics {
  public delegate T Filter<T>(T value, int x, int y);

  public static class Filters {
    public static void ApplyFilter<T>(this IGrid<T> grid, Filter<T> filter) {
      for (var y = 0; y < grid.Height; y++)
      for (var x = 0; x < grid.Width; x++) {
        var value = grid[x, y];

        grid[x, y] = filter(value, x, y);
      }
    }

    public static Filter<T> Constant<T>(T constant) {
      return (_, x, y) => constant;
    }

    public static Filter<T> ToFilter<T>(this IGrid<T> grid) {
      return (value, x, y) => grid[x, y];
    }

    public static Filter<T> CellShade<T>(this Filter<bool> filter, T shade) {
      return (value, x, y) => filter(default, x, y) ? shade : value;
    }

    public static Filter<T> CellShade<T>(this Filter<T> filter, T shade, Func<T, bool> examiner) {
      return (value, x, y) => examiner(filter(default!, x, y)) ? shade : value;
    }

    public static Filter<T> Offset<T>(this Filter<T> filter, Vector2 offset) {
      var range = Range.Of(0f, 15f);

      return (value, x, y) => filter(value,
          (int) (x + offset.X).Clamp(range),
          (int) (y + offset.Y).Clamp(range)
      );
    }

    public static Filter<T> Masked<T>(this Filter<T> filter, Rectangle mask) {
      return (value, x, y) => {
        var point = new Vector2(x, y);

        if (mask.Contains(point)) {
          return filter(value, x, y);
        }

        return value;
      };
    }

    public static Filter<Color> Outlined(this Filter<Color> filter, Color outlineColor) {
      return (value, x, y) => {
        var pixelLeft  = filter(value, x    - 1, y);
        var pixelDown  = filter(value, x, y - 1);
        var pixelRight = filter(value, x    + 1, y);
        var pixelUp    = filter(value, x, y + 1);

        if (Math.Abs(pixelUp.A * pixelDown.A * pixelLeft.A * pixelRight.A) < float.Epsilon) {
          return outlineColor;
        }

        return filter(value, x, y);
      };
    }

    public static Filter<T> Scaled<T>(this Filter<T> filter, int scale) {
      return (value, x, y) => filter(value, x / scale, y / scale);
    }

    public static Filter<float> Threshold(int level) {
      return (value, x, y) => (float) (Math.Round(value * level) / level);
    }

    public static Filter<T> Mirror<T>(this Filter<T> filter, Axis axis, Vector2I size, Vector2I splitPoint) {
      if (axis.HasFlagFast(Axis.Horizontal)) {
        filter = filter.MirrorHorizontally(size.X, splitPoint.X);
      }

      if (axis.HasFlagFast(Axis.Vertical)) {
        filter = filter.MirrorVertically(size.Y, splitPoint.Y);
      }

      return filter;
    }

    public static Filter<T> MirrorVertically<T>(this Filter<T> filter, int size, int splitPoint) {
      return (value, x, y) => {
        if (x < splitPoint) {
          return filter(value, x, y);
        }

        return filter(value, size - x - 1, y);
      };
    }

    public static Filter<T> MirrorHorizontally<T>(this Filter<T> filter, int height, int splitPoint) {
      return (value, x, y) => {
        if (y < splitPoint) {
          return filter(value, x, y);
        }

        return filter(value, x, height - y - 1);
      };
    }
  }
}