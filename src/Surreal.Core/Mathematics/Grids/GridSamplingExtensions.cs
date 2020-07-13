using System;
using Surreal.Mathematics.Linear;

namespace Surreal.Mathematics.Grids {
  public static class GridSamplingExtensions {
    public static bool Contains<T>(this IGrid<T> grid, int x, int y) {
      if (x >= 0 && x < grid.Width &&
          y >= 0 && y < grid.Height) {
        return true;
      }

      return false;
    }

    public static T TryGet<T>(this IGrid<T> grid, int x, int y, T defaultValue = default) {
      if (grid.Contains(x, y)) {
        return grid[x, y];
      }

      return defaultValue;
    }

    public static int CountNeighbours<TNeighbourhood>(this IGrid<bool> grid, Vector2I position)
        where TNeighbourhood : struct, INeighbourhood {
      var count = 0;

      foreach (var (x, y) in position.GetNeighbourhood<TNeighbourhood>()) {
        if (grid.Contains(x, y)) {
          count += grid[x, y] ? 1 : 0;
        }
      }

      return count;
    }

    public static int CountNeighbours<T, TNeighbourhood>(this IGrid<T> grid, Vector2I position, Func<T, bool> examiner)
        where TNeighbourhood : struct, INeighbourhood {
      var count = 0;

      foreach (var (x, y) in position.GetNeighbourhood<TNeighbourhood>()) {
        if (grid.Contains(x, y)) {
          count += examiner(grid[x, y]) ? 1 : 0;
        }
      }

      return count;
    }

    public static void Blit<T>(this IGrid<T> target, IGrid<T> source, int offsetX, int offsetY, int width, int height) {
      for (var y = 0; y < source.Height; y++) {
        var positionY = y + offsetY;
        if (positionY < 0 || positionY >= target.Height) continue;

        for (var x = 0; x < source.Width; x++) {
          var positionX = x + offsetX;
          if (positionX < 0 || positionX >= target.Width) continue;

          target[positionX, positionY] = source[x, y];
        }
      }
    }

    public static T Sample<T>(this IGrid<T> grid, int x, int y, GridSamplingMode mode) {
      switch (mode) {
        case GridSamplingMode.Clamp:
          if (x < 0) x               = 0;
          if (y < 0) y               = 0;
          if (x > grid.Width  - 1) x = grid.Width  - 1;
          if (y > grid.Height - 1) y = grid.Height - 1;
          break;

        case GridSamplingMode.ClampToDefault:
          if (x < 0) return default!;
          if (y < 0) return default!;
          if (x > grid.Width  - 1) return default!;
          if (y > grid.Height - 1) return default!;
          break;

        case GridSamplingMode.Wrap:
          if (x < 0) x               = grid.Width  - 1;
          if (y < 0) y               = grid.Height - 1;
          if (x > grid.Width  - 1) x = 0;
          if (y > grid.Height - 1) y = 0;
          break;

        default:
          throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
      }

      return grid[x, y];
    }
  }
}