namespace Surreal.Mathematics;

/// <summary>
/// A frustum is a geometric shape that defines the viewing volume of a camera.
/// </summary>
public readonly record struct Frustum(Plane[] Planes)
{
  /// <summary>
  /// Builds a frustum from a projection matrix using the Gribb/Hartmann algorithm.
  /// </summary>
  public static Frustum FromProjectionMatrix(in Matrix4x4 projection)
  {
    var left = new Plane(projection.M14 + projection.M11, projection.M24 + projection.M21, projection.M34 + projection.M31, projection.M44 + projection.M41);
    var right = new Plane(projection.M14 - projection.M11, projection.M24 - projection.M21, projection.M34 - projection.M31, projection.M44 - projection.M41);
    var bottom = new Plane(projection.M14 + projection.M12, projection.M24 + projection.M22, projection.M34 + projection.M32, projection.M44 + projection.M42);
    var top = new Plane(projection.M14 - projection.M12, projection.M24 - projection.M22, projection.M34 - projection.M32, projection.M44 - projection.M42);
    var near = new Plane(projection.M13, projection.M23, projection.M33, projection.M43);
    var far = new Plane(projection.M14 - projection.M13, projection.M24 - projection.M23, projection.M34 - projection.M33, projection.M44 - projection.M43);

    return new Frustum([left, right, bottom, top, near, far]);
  }

  /// <summary>
  /// Determines if the given point is contained within the frustum.
  /// </summary>
  public bool ContainsPoint(Vector3 point)
  {
    for (int i = 0; i < Planes.Length; i++)
    {
      var plane = Planes[i];
      if (plane.DistanceTo(point) < 0)
      {
        return false;
      }
    }

    return true;
  }

  /// <summary>
  /// Determines if the given sphere is contained within the frustum.
  /// </summary>
  public bool ContainsSphere(Vector3 center, float radius)
  {
    for (int i = 0; i < Planes.Length; i++)
    {
      var plane = Planes[i];
      if (plane.DistanceTo(center) < -radius)
      {
        return false;
      }
    }

    return true;
  }
}
