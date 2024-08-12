namespace Surreal.Mathematics;

/// <summary>
/// Helpers for working with planes.
/// </summary>
public static class Planes
{
  /// <summary>
  /// Calculates the distance from the plane to the given point.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float DistanceTo(this Plane plane, Vector3 point)
    => Vector3.Dot(plane.Normal, point) + plane.D;
}
