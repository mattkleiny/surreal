using System;
using System.Numerics;
using System.Threading;
using Surreal.Mathematics.Linear;

namespace Surreal.Mathematics
{
  public static class Maths
  {
    private static readonly ThreadLocal<Random> ThreadLocalRandom = new(() => new Random(Environment.TickCount));

    public const float E   = MathF.E;
    public const float Pi  = MathF.PI;
    public const float Tau = MathF.Tau;

    public static Random Random => ThreadLocalRandom.Value!;

    public static Point2  P(int x, int y)                       => new(x, y);
    public static Point3  P(int x, int y, int z)                => new(x, y, z);
    public static Vector2 V(float x, float y)                   => new(x, y);
    public static Vector3 V(float x, float y, float z)          => new(x, y, z);
    public static Vector4 V(float x, float y, float z, float w) => new(x, y, z, w);

    public static float   NextFloat(this Random random)                       => (float) random.NextDouble();
    public static float   NextFloat(this Random random, float min, float max) => random.NextFloat() * (max - min) + min;
    public static int     NextSign(this Random random)                        => random.Next() < 0.5f ? 1 : -1;
    public static bool    NextBool(this Random random, float chance = 0.5f)   => random.NextDouble() < chance;
    public static bool    NextChance(this Random random, float chance)        => NextBool(random, chance);
    public static int     NextRange(this Random random, IntRange range)       => random.Next(range.Min, range.Max);
    public static float   NextRange(this Random random, FloatRange range)     => random.NextFloat(range.Min, range.Max);
    public static Seed    NextSeed(this Random random)                        => new(random.Next());
    public static Vector2 NextUnitCircle(this Random random)                  => new(random.NextFloat(-1, 1f), random.NextFloat(-1f, 1f));
    public static Vector3 NextUnitSphere(this Random random)                  => new(random.NextFloat(-1, 1f), random.NextFloat(-1f, 1f), random.NextFloat(-1, 1f));

    public static float DegreesToRadians(float degrees) => (float) (degrees * (Math.PI / 180));
    public static float RadiansToDegrees(float radians) => (float) (radians * (180 / Math.PI));

    public static float   Fract(float value)   => value % 1;
    public static Vector2 Fract(Vector2 value) => new(Fract(value.X), Fract(value.Y));
    public static Vector3 Fract(Vector3 value) => new(Fract(value.X), Fract(value.Y), Fract(value.Z));
    public static Vector4 Fract(Vector4 value) => new(Fract(value.X), Fract(value.Y), Fract(value.Z), Fract(value.W));

    public static int   Lerp(int a, int b, float t)     => (int) (a + t * (b - a));
    public static float Lerp(float a, float b, float t) => a + t * (b - a);

    public static int Clamp(int value, int lower, int upper)
    {
      if (value < lower) return lower;
      if (value > upper) return upper;

      return value;
    }

    public static float Clamp(float value, float lower, float upper)
    {
      if (value < lower) return lower;
      if (value > upper) return upper;

      return value;
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
}