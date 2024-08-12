namespace Surreal.Mathematics;

/// <summary>
/// Common used mathematical utilities.
/// </summary>
public static class MathE
{
  /// <summary>
  /// Converts degrees to radians.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float DegreesToRadians(float degrees)
  {
    return (float)(degrees * (Math.PI / 180));
  }

  /// <summary>
  /// Converts radians to degrees.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float RadiansToDegrees(float radians)
  {
    return (float)(radians * (180 / Math.PI));
  }

  /// <summary>
  /// Interpolates between two values.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int Lerp(int a, int b, float t)
  {
    return (int)(a + t * (b - a));
  }

  /// <summary>
  /// Interpolates between two values.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float Lerp(float a, float b, float t)
  {
    return a + t * (b - a);
  }

  /// <summary>
  /// Interpolates between two values.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static double Lerp(double a, double b, float t)
  {
    return a + t * (b - a);
  }

  /// <summary>
  /// Interpolates between two values.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Lerp<T>(T a, T b, float t)
    where T : IFromLerp<T>
  {
    return T.Lerp(a, b, t);
  }

  /// <summary>
  /// Ping pongs a value between 0 and 1.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float PingPong(float t)
  {
    return (MathF.Sin(t) + 1f) / 2f;
  }

  /// <summary>
  /// Rounds a number to the nearest integer.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int CeilToInt(float value)
  {
    return (int)MathF.Ceiling(value);
  }

  /// <summary>
  /// Rounds a number to the nearest integer.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int FloorToInt(float value)
  {
    return (int)MathF.Floor(value);
  }

  /// <summary>
  /// Rounds a number to the nearest integer.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int RoundToInt(float value)
  {
    return (int)MathF.Round(value);
  }

  /// <summary>
  /// Clamp a value between 0 and 1.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Clamp01<T>(T value)
    where T : INumber<T>
  {
    return T.Clamp(value, T.Zero, T.One);
  }

  /// <summary>
  /// Clamp a value between -1 and 1.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Clamp11<T>(T value)
    where T : INumber<T>
  {
    return T.Clamp(value, -T.One, T.One);
  }

  /// <summary>
  /// Wraps a value between a lower and upper bound.
  /// </summary>
  public static T Wrap<T>(T value, T lower, T upper)
    where T : INumber<T>
  {
    if (value < lower)
    {
      return upper - (lower - value) % (upper - lower);
    }

    return lower + (value - lower) % (upper - lower);
  }
}
