using System.Runtime.CompilerServices;

namespace Asteroids.Actors;

/// <summary>A simple polygon in 2-space that can be transformed and translated.</summary>
public sealed class Polygon
{
  private readonly Vector2[] vertices;

  /// <summary>Creates a new randomly shaped <see cref="Polygon"/>.</summary>
  public static Polygon Create()
  {
    // TODO: randomly generate me

    return new Polygon(
      new Vector2(8, 3),
      new Vector2(11, 5),
      new Vector2(15, 10),
      new Vector2(17, 13),
      new Vector2(15, 16),
      new Vector2(11, 18),
      new Vector2(7, 18),
      new Vector2(0, 12)
    );
  }

  private Polygon(params Vector2[] vertices)
  {
    this.vertices = vertices;
  }

  /// <summary>The center of the polygon in 2-space.</summary>
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

  /// <summary>The size of the polygon in 2-space.</summary>
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

  /// <summary>Determines if the polygon contains the given point.</summary>
  public bool ContainsPoint(Vector2 point)
  {
    var direction = point with { X = 1000f };
    var intersectionCount = 0;

    for (var i = 0; i < vertices.Length; i++)
    {
      // for each edge in the polygon
      var start = vertices[i];
      var end = i == vertices.Length - 1
        ? vertices[0] // wind around end of polygon
        : vertices[i + 1];

      // determine the number of times our 'ray' intersects the polygon
      if (Intersect(point, direction, start, end))
      {
        intersectionCount++;
      }
    }

    return intersectionCount % 2 == 1; // if there where an odd amount of intersection, the point lies within the polygon
  }

  /// <summary>Translates a polygon by the given amount.</summary>
  public void Translate(Vector2 amount)
  {
    var matrix = Matrix3x2.CreateTranslation(amount);

    for (var i = 0; i < vertices.Length; i++)
    {
      ref var vertex = ref vertices[i];

      vertex = Vector2.Transform(vertex, matrix);
    }
  }

  /// <summary>Rotates a polygon by the given amount.</summary>
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
    // checks if three points are listed in counter-clockwise order.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsCounterClockwise(Vector2 a, Vector2 b, Vector2 c)
    {
      return (c.Y - a.Y) * (b.X - a.X) > (b.Y - a.Y) * (c.X - a.X);
    }

    return IsCounterClockwise(a, c, d) != IsCounterClockwise(b, c, d) &&
           IsCounterClockwise(a, b, c) != IsCounterClockwise(a, b, d);
  }
}
