using System;
using System.Numerics;
using Prelude.Core;
using Surreal.Framework.Tiles;
using Surreal.Graphics;
using Surreal.Graphics.Textures;
using Surreal.Mathematics.Grids;
using Surreal.Mathematics.Linear;

namespace Prelude.Graphics {
  public static class RaycastExtensions {
    public static Vector2 CastRay(this TileMap<Tile> map, Ray ray) {
      var (position, direction) = ray;
      var slope = direction.X / direction.Y;

      do {
        float edgeDistanceX;
        float edgeDistanceY;

        if (direction.X > 0f) {
          edgeDistanceX = MathF.Floor(position.X) + 1 - position.X;
        } else {
          edgeDistanceX = MathF.Ceiling(position.X) - 1 - position.X;
        }

        if (direction.Y > 0f) {
          edgeDistanceY = MathF.Floor(position.Y) + 1 - position.Y;
        } else {
          edgeDistanceY = MathF.Ceiling(position.Y) - 1 - position.Y;
        }

        var step1 = new Vector2(edgeDistanceX, edgeDistanceX / slope);
        var step2 = new Vector2(edgeDistanceY * slope, edgeDistanceY);

        if (step1.LengthSquared() < step2.LengthSquared()) {
          position += step1;
        } else {
          position += step2;
        }
      } while (!map.GetNearestTile(new Ray(position, direction)).IsSolid);

      return position;
    }

    public static Tile GetNearestTile(this TileMap<Tile> grid, Ray ray) {
      var (origin, direction) = ray;

      var offsetX = 0f;
      var offsetY = 0f;

      if (Math.Abs(MathF.Floor(origin.X) - origin.X) < float.Epsilon) {
        offsetX = direction.X > 0 ? 0 : -1;
      }

      if (Math.Abs(MathF.Floor(origin.Y) - origin.Y) < float.Epsilon) {
        offsetY = direction.Y > 0 ? 0 : -1;
      }

      var x = (int) (origin.X + offsetX);
      var y = (int) (origin.Y + offsetY);

      return grid[x, y];
    }

    public static void DrawColoredColumn(
        this ImageRegion target,
        int sourceX,
        Vector2 point,
        float height,
        Color color) {
      var start = (int) point.Y;
      var end   = (int) MathF.Ceiling(point.Y + height);

      for (var y = Math.Max(0, start); y < Math.Min(target.Height, end); y++) {
        target[(int) point.X, y] = color;
      }
    }

    public static void DrawTexturedColumn(
        this ImageRegion target,
        int sourceX,
        ImageRegion texture,
        Vector2 point,
        float height,
        Color dampenColor) {
      var start = (int) point.Y;
      var end   = (int) MathF.Ceiling(point.Y + height);
      var stepY = texture.Height / height;

      for (var y = Math.Max(0, start); y < Math.Min(target.Height, end); y++) {
        var sourceY     = (y - point.Y) * stepY;
        var sourceColor = texture[sourceX, (int) sourceY];

        target[(int) point.X, y] = sourceColor - dampenColor;
      }
    }
  }
}