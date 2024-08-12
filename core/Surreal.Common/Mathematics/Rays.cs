namespace Surreal.Mathematics;

/// <summary>
/// A ray in 3-space, composed of an origin and direction.
/// </summary>
public readonly record struct Ray(Vector3 Origin, Vector3 Direction)
{
  /// <summary>
  /// Calculates the point at the given distance along the ray.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Vector3 At(float distance) => Origin + distance * Direction;
}