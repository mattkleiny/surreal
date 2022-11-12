namespace Surreal.Mathematics;

/// <summary>Common used mathematical utilities.</summary>
public static class Maths
{
  public static float DegreesToRadians(float degrees)
  {
    return (float) (degrees * (Math.PI / 180));
  }

  public static float RadiansToDegrees(float radians)
  {
    return (float) (radians * (180 / Math.PI));
  }

  public static int Lerp(int a, int b, float t)
  {
    return (int) (a + t * (b - a));
  }

  public static float Lerp(float a, float b, float t)
  {
    return a + t * (b - a);
  }

  public static float PingPong(float t)
  {
    return (MathF.Sin(t) + 1f) / 2f;
  }

  public static int CeilToInt(float value)
  {
    return (int) MathF.Ceiling(value);
  }

  public static int FloorToInt(float value)
  {
    return (int) MathF.Floor(value);
  }

  public static int RoundToInt(float value)
  {
    return (int) MathF.Round(value);
  }

  public static int Wrap(int value, int lower, int upper)
  {
    if (value < lower)
    {
      return upper - (lower - value) % (upper - lower);
    }

    return lower + (value - lower) % (upper - lower);
  }
}



