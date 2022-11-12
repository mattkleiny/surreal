using System.Runtime.CompilerServices;

namespace Surreal.Actors;

/// <summary>A simple polygon in 2-space that can be transformed and queried.</summary>
public sealed class Polygon
{
  private Vector2[] _vertices;

  /// <summary>Creates an empty polygon.</summary>
  public Polygon()
    : this(Array.Empty<Vector2>())
  {
  }

  /// <summary>Creates a polygon with the given vertices.</summary>
  public Polygon(Vector2[] vertices)
  {
    _vertices = vertices;
  }

  public int Length => _vertices.Length;

  public Vector2 Center => Bounds.Center;
  public Vector2 Size => Bounds.Size;

  /// <summary>The bounds of the polygon in 2-space.</summary>
  public Rectangle Bounds
  {
    get
    {
      var minX = float.MaxValue;
      var minY = float.MaxValue;
      var maxX = 0f;
      var maxY = 0f;

      foreach (var vertex in _vertices)
      {
        if (vertex.X < minX)
        {
          minX = vertex.X;
        }

        if (vertex.Y < minY)
        {
          minY = vertex.Y;
        }

        if (vertex.X > maxX)
        {
          maxX = vertex.X;
        }

        if (vertex.Y > maxY)
        {
          maxY = vertex.Y;
        }
      }

      return new Rectangle(minX, maxY, maxX, minY);
    }
  }

  /// <summary>Creates a new polygon to represent a triangle.</summary>
  public static Polygon CreateTriangle(float scale)
  {
    var vertices = new Vector2[3];

    vertices[0] = new Vector2(-scale, scale);
    vertices[1] = new Vector2(0f, -scale);
    vertices[2] = new Vector2(scale, scale);

    return new Polygon(vertices);
  }

  /// <summary>Creates a new randomly shaped polygon to represent an asteroid.</summary>
  public static Polygon CreateAsteroid(FloatRange radiusRange)
  {
    var random = Random.Shared;

    var vertices = new Vector2[random.NextInt(5, 12)];
    var list = new SpanList<Vector2>(vertices);

    var theta = 0f;

    for (var i = 0; i < list.Capacity; i++)
    {
      theta += 2 * MathF.PI / list.Capacity;

      var radius = random.NextRange(radiusRange);

      var x = radius * MathF.Cos(theta);
      var y = radius * MathF.Sin(theta);

      list.Add(new Vector2(x, y));
    }

    return new Polygon(vertices);
  }

  /// <summary>Determines if the polygon contains the given point.</summary>
  public bool ContainsPoint(Vector2 point)
  {
    var direction = point with { X = 1000f };
    var intersectionCount = 0;

    for (var i = 0; i < _vertices.Length; i++)
    {
      // for each edge in the polygon
      var start = _vertices[i];
      var end = i == _vertices.Length - 1
        ? _vertices[0] // wind around end of polygon
        : _vertices[i + 1];

      // determine the number of times our 'ray' intersects the polygon
      if (Intersect(point, direction, start, end))
      {
        intersectionCount++;
      }
    }

    return intersectionCount % 2 == 1; // if there where an odd amount of intersection, the point lies within the polygon
  }

  /// <summary>Transforms this polygon from the given other polygon by the given matrix.</summary>
  public void TransformFrom(Polygon other, in Matrix4x4 transform)
  {
    if (Length != other.Length)
    {
      Array.Resize(ref _vertices, other.Length);
    }

    for (var i = 0; i < Length; i++) _vertices[i] = Vector2.Transform(other._vertices[i], transform);
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


