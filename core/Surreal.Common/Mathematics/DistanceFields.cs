using System.Runtime.CompilerServices;
using Surreal.Memory;

namespace Surreal.Mathematics;

/// <summary>Utilities for generating distance fields.</summary>
public static class DistanceFields
{
  /// <summary>Generates a distance field on the given grid of <see cref="Color32"/>.</summary>
  public static void GenerateInPlace(SpanGrid<Color32> grid, int spread = 8)
  {
    GenerateInPlace(
      grid: grid,
      isInside: color => (color.R >= 128 || color.G >= 128 || color.B >= 128) && color.A >= 128,
      factory: alpha => new Color(alpha, alpha, alpha)
    );
  }

  /// <summary>Generates a distance field on the given grid of <see cref="T"/>.</summary>
  public static void GenerateInPlace<T>(SpanGrid<T> grid, Predicate<T> isInside, Func<float, T> factory, int spread = 8)
  {
    var bitmap = new Grid<bool>(grid.Width, grid.Height);

    // compute bitmap for shape edges
    for (var y = 0; y < grid.Height; y++)
    for (var x = 0; x < grid.Width; x++)
    {
      bitmap[x, y] = isInside(grid[x, y]);
    }

    // compute distance to nearest edge
    for (var y = 0; y < grid.Height; y++)
    for (var x = 0; x < grid.Width; x++)
    {
      var distance = FindSignedDistance(bitmap, x, y, spread);
      var alpha = DistanceToAlpha(distance, spread);

      grid[x, y] = factory(alpha);
    }
  }

  /// <summary>Computes the signed distance to the nearest edge in a given <see cref="bitmap"/>.</summary>
  private static float FindSignedDistance(Grid<bool> bitmap, int centerX, int centerY, int spread)
  {
    var width = bitmap.Width;
    var height = bitmap.Height;

    var baseVal = bitmap[centerX, centerY];

    var startX = Math.Max(0, centerX - spread);
    var endX = Math.Min(width - 1, centerX + spread);

    var startY = Math.Max(0, centerY - spread);
    var endY = Math.Min(height - 1, centerY + spread);

    var closestSquareDist = spread * spread;

    for (var y = startY; y <= endY; ++y)
    for (var x = startX; x <= endX; ++x)
    {
      if (baseVal != bitmap[x, y])
      {
        var squareDist = SquareDist(centerX, centerY, x, y);
        if (squareDist < closestSquareDist)
        {
          closestSquareDist = squareDist;
        }
      }
    }

    var closestDist = MathF.Sqrt(closestSquareDist);
    var sign = baseVal ? 1 : -1;

    return sign * Math.Min(closestDist, spread);
  }

  /// <summary>Computes the equivalent alpha representation as a fall-off based on <see cref="signedDistance"/>.</summary>
  private static float DistanceToAlpha(float signedDistance, int spread)
  {
    var alpha = 0.5f + 0.5f * (signedDistance / spread);

    return alpha.Clamp(0f, 1f);
  }

  /// <summary>Computes square distance between two integral points.</summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static int SquareDist(int x1, int y1, int x2, int y2)
  {
    var dx = x1 - x2;
    var dy = y1 - y2;

    return dx * dx + dy * dy;
  }
}
