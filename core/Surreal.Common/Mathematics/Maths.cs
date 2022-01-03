namespace Surreal.Mathematics;

/// <summary>Common used mathematical utilities.</summary>
public static class Maths
{
  public static float   NextFloat(this Random random)                       => (float) random.NextDouble();
  public static float   NextFloat(this Random random, float min, float max) => random.NextFloat() * (max - min) + min;
  public static int     NextSign(this Random random)                        => random.Next() < 0.5f ? 1 : -1;
  public static bool    NextBool(this Random random, float chance = 0.5f)   => random.NextDouble() < chance;
  public static bool    NextChance(this Random random, float chance)        => NextBool(random, chance);
  public static int     NextRange(this Random random, IntRange range)       => random.Next(range.Min, range.Max);
  public static float   NextRange(this Random random, FloatRange range)     => random.NextFloat(range.Min, range.Max);
  public static Seed    NextSeed(this Random random)                        => new(random.Next());
  public static Color   NextColor(this Random random)                       => new(random.NextFloat(), random.NextFloat(), random.NextFloat());
  public static Vector2 NextUnitCircle(this Random random)                  => new(random.NextFloat(-1, 1f), random.NextFloat(-1f, 1f));

  public static float DegreesToRadians(float degrees) => (float) (degrees * (Math.PI / 180));
  public static float RadiansToDegrees(float radians) => (float) (radians * (180 / Math.PI));

  public static int   Lerp(int a, int b, float t)     => (int) (a + t * (b - a));
  public static float Lerp(float a, float b, float t) => a + t * (b - a);

  public static float Repeat(float t, float length)
  {
    return (t - MathF.Floor(t / length) * length).Clamp(0.0f, length);
  }

  public static float PingPong(float t, float length)
  {
    t = Repeat(t, length * 2f);

    return length - MathF.Abs(t - length);
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
