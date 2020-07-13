using System;
using System.Text;
using Surreal.Graphics;
using Surreal.Graphics.Textures;
using Surreal.Mathematics.Grids;

namespace Surreal.Diagnostics {
  public static class VisualizationExtensions {
    public static string ToString<T>(this IGrid<T> grid, Func<int, int, T, char> painter) {
      var builder = new StringBuilder();

      for (var y = 0; y < grid.Height; y++) {
        if (y > 0) builder.AppendLine();

        for (var x = 0; x < grid.Width; x++) {
          var tile = grid[x, y];
          var rune = painter(x, y, tile);

          builder.Append(rune);
        }
      }

      return builder.ToString();
    }

    public static Image ToImage<T>(this IGrid<T> grid, Func<int, int, T, Color> painter, int scale = 1) {
      var image = new Image(grid.Width * scale, grid.Height * scale);

      for (var y = 0; y < grid.Height; y++)
      for (var x = 0; x < grid.Width; x++) {
        var tile  = grid[x, y];
        var color = painter(x, y, tile);

        for (var yy = 0; yy < scale; yy++)
        for (var xx = 0; xx < scale; xx++) {
          image[x * scale + xx, y * scale + yy] = color;
        }
      }

      return image;
    }
  }
}