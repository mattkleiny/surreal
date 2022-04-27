using System.Runtime.CompilerServices;
using Surreal.Memory;

namespace Asteroids.Actors;

/// <summary>A simple polygon in 2-space that can be transformed and translated.</summary>
public sealed class Polygon
{
  private Vector2[] vertices;

  /// <summary>Creates an empty polygon.</summary>
  public Polygon()
    : this(Array.Empty<Vector2>())
  {
  }

  /// <summary>Creates a polygon with the given vertices.</summary>
  public Polygon(params Vector2[] vertices)
  {
    this.vertices = vertices;
  }

  public int Length => vertices.Length;

  public Vector2 Center => Bounds.Center;
  public Vector2 Size   => Bounds.Size;

  /// <summary>The bounds of the polygon in 2-space.</summary>
  public BoundingRect Bounds
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

      return new BoundingRect(minX, maxY, maxX, minY);
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

  /// <summary>Transforms the given other polygon by the given matrix and writes the results to this polygon.</summary>
  public void TransformFrom(Polygon other, in Matrix4x4 transform)
  {
    if (Length != other.Length)
    {
      Array.Resize(ref vertices, other.Length);
    }

    for (var i = 0; i < Length; i++)
    {
      vertices[i] = Vector2.Transform(other.vertices[i], transform);
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
