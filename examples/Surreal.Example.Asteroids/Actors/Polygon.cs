using System.Runtime.CompilerServices;

namespace Asteroids.Actors;

public enum PolygonSize
{
  Small,
  Medium,
  Large
}

public sealed class Polygon
{
  private readonly Vector2[] vertices;

  public static Polygon Create()
  {
    var random = Random.Shared;
    var size = random.NextEnum<PolygonSize>();

    // TODO: clean this up
    return size switch
    {
      PolygonSize.Large => random.NextBool()
        ? new(
          new(8, 3),
          new(11, 5),
          new(15, 10),
          new(17, 13),
          new(15, 16),
          new(11, 18),
          new(7, 18),
          new(0, 12)
        )
        : new(
          new(3, 4),
          new(10, 2),
          new(16, 5),
          new(15, 11),
          new(16, 16),
          new(6, 18),
          new(0, 14),
          new(4, 9)
        ),
      PolygonSize.Medium => random.NextBool()
        ? new(
          new(1, 2),
          new(5, 1),
          new(9, 4),
          new(4, 9),
          new(0, 5)
        )
        : new(
          new(2, 2),
          new(6, 0),
          new(9, 3),
          new(6, 6),
          new(7, 11),
          new(1, 10),
          new(0, 5)
        ),
      PolygonSize.Small => random.NextBool()
        ? new(
          new(3, 1),
          new(5, 4),
          new(4, 5),
          new(2, 6),
          new(0, 3)
        )
        : new(
          new(2, 1),
          new(4, 0),
          new(7, 3),
          new(4, 5),
          new(0, 4)
        ),
      _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
    };
  }

  private Polygon(params Vector2[] vertices)
  {
    this.vertices = vertices;
  }

  public Vector2 Center
  {
    get
    {
      var maxX = 0f;
      var maxY = 0f;

      foreach (var vertex in vertices)
      {
        if (vertex.X > maxX) maxX = vertex.X;
        if (vertex.Y > maxY) maxY = vertex.Y;
      }

      return new Vector2(maxX, maxY) / 2f;
    }
  }

  public Vector2 Size
  {
    get
    {
      var minX = float.MaxValue;
      var minY = float.MaxValue;
      var maxX = 0f;
      var maxY = 0f;

      foreach (var vertex in vertices)
      {
        if (vertex.X < minX) minX = vertex.X;
        if (vertex.Y < minY) minY = vertex.Y;
        if (vertex.X > maxX) maxX = vertex.X;
        if (vertex.Y > maxY) maxY = vertex.Y;
      }

      return new Vector2(maxX - minX, maxY - minY);
    }
  }

  public bool ContainsPoint(Vector2 point)
  {
    var count = 0;
    var extreme = point with { X = 1000f };

    for (var i = 0; i < vertices.Length; i++)
    {
      var start = vertices[i];
      var end = i == vertices.Length - 1
        ? vertices[0] // wind around end of polygon
        : vertices[i + 1];

      if (Intersect(point, extreme, start, end))
      {
        count++;
      }
    }

    return count % 2 == 1; // if there where an odd amount of intersection, the point lies within the polygon
  }

  public void Translate(Vector2 amount)
  {
    var matrix = Matrix3x2.CreateTranslation(amount);

    for (var i = 0; i < vertices.Length; i++)
    {
      ref var vertex = ref vertices[i];

      vertex = Vector2.Transform(vertex, matrix);
    }
  }

  public void Rotate(float angle)
  {
    var quaternion = Quaternion.CreateFromYawPitchRoll(0f, 0f, angle);

    for (var i = 0; i < vertices.Length; i++)
    {
      ref var vertex = ref vertices[i];

      vertex = Vector2.Transform(vertex, quaternion);
    }
  }

  /// <summary>Determines if two lines intersect (ignoring co-linearity in this case).</summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool Intersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
  {
    return CCW(a, c, d) != CCW(b, c, d) && CCW(a, b, c) != CCW(a, b, d);
  }

  /// <summary>Checks if three points are listed in counter-clockwise order</summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool CCW(Vector2 a, Vector2 b, Vector2 c)
  {
    return (c.Y - a.Y) * (b.X - a.X) > (b.Y - a.Y) * (c.X - a.X);
  }
}
