using System;
using System.Numerics;
using Surreal.Framework.Tiles;
using Surreal.Mathematics.Grids;
using Surreal.Mathematics.Linear;

namespace Surreal.Graphics.Raycasting {
  // TODO: clean this up (whew)

  public static class RaycastExtensions {
    public static Vector2 CastRay<TTile>(this TileMap<TTile> map, Ray ray)
        where TTile : IRaycastAwareTile {
      var (position, direction) = ray;
      var slope = direction.X / direction.Y;

      do {
        float edgeDistanceX;
        float edgeDistanceY;

        if (direction.X > 0f) {
          edgeDistanceX = System.MathF.Floor(position.X) + 1 - position.X;
        }
        else {
          edgeDistanceX = System.MathF.Ceiling(position.X) - 1 - position.X;
        }

        if (direction.Y > 0f) {
          edgeDistanceY = System.MathF.Floor(position.Y) + 1 - position.Y;
        }
        else {
          edgeDistanceY = System.MathF.Ceiling(position.Y) - 1 - position.Y;
        }

        var step1 = new Vector2(edgeDistanceX, edgeDistanceX / slope);
        var step2 = new Vector2(edgeDistanceY                * slope, edgeDistanceY);

        if (step1.LengthSquared() < step2.LengthSquared()) {
          position += step1;
        }
        else {
          position += step2;
        }
      } while (!map.GetNearestTile(new Ray(position, direction)).IsSolid);

      return position;
    }

    public static TTile GetNearestTile<TTile>(this IGrid<TTile> grid, Ray ray) {
      var (origin, direction) = ray;

      var offsetX = 0f;
      var offsetY = 0f;

      if (Math.Abs(System.MathF.Floor(origin.X) - origin.X) < float.Epsilon) {
        offsetX = direction.X > 0 ? 0 : -1;
      }

      if (Math.Abs(System.MathF.Floor(origin.Y) - origin.Y) < float.Epsilon) {
        offsetY = direction.Y > 0 ? 0 : -1;
      }

      var x = (int) (origin.X + offsetX);
      var y = (int) (origin.Y + offsetY);

      return grid[x, y];
    }

    public static void DrawColoredColumn(this IGrid<Color> target, int sourceX, Vector2 point, float height, Color color) {
      var start = (int) point.Y;
      var end   = (int) System.MathF.Ceiling(point.Y + height);

      for (var y = Math.Max(0, start); y < Math.Min(target.Height, end); y++) {
        target[(int) point.X, y] = color;
      }
    }

    public static void DrawTexturedColumn(this IGrid<Color> target, int sourceX, IGrid<Color> texture, Vector2 point, float height, Color dampenColor) {
      var start = (int) point.Y;
      var end   = (int) System.MathF.Ceiling(point.Y + height);
      var stepY = texture.Height / height;

      for (var y = Math.Max(0, start); y < Math.Min(target.Height, end); y++) {
        var sourceY     = (y - point.Y) * stepY;
        var sourceColor = texture[sourceX, (int) sourceY];

        target[(int) point.X, y] = sourceColor - dampenColor;
      }
    }
  }
}