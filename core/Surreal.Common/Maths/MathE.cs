namespace Surreal.Maths;

/// <summary>
/// Represents a value that can be interpolated between two values.
/// </summary>
public interface IInterpolated<T>
{
  /// <summary>
  /// Interpolates between two values.
  /// </summary>
  static abstract T Lerp(T a, T b, float t);
}

/// <summary>
/// Common used mathematical utilities.
/// </summary>
public static class MathE
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float DegreesToRadians(float degrees)
    => (float)(degrees * (Math.PI / 180));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float RadiansToDegrees(float radians)
    => (float)(radians * (180 / Math.PI));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int Lerp(int a, int b, float t)
    => (int)(a + t * (b - a));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float Lerp(float a, float b, float t)
    => a + t * (b - a);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static double Lerp(double a, double b, float t)
    => a + t * (b - a);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Lerp<T>(T a, T b, float t)
    where T : IInterpolated<T> => T.Lerp(a, b, t);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float PingPong(float t)
    => (MathF.Sin(t) + 1f) / 2f;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int CeilToInt(float value)
    => (int)MathF.Ceiling(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int FloorToInt(float value)
    => (int)MathF.Floor(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int RoundToInt(float value)
    => (int)MathF.Round(value);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int Wrap(int value, int lower, int upper)
  {
    if (value < lower)
    {
      return upper - (lower - value) % (upper - lower);
    }

    return lower + (value - lower) % (upper - lower);
  }
}
