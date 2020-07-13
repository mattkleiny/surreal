using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Surreal.Mathematics;
using Surreal.Mathematics.Linear;

namespace Surreal {
  public static class Maths {
    private static readonly ThreadLocal<Random> ThreadLocalRandom = new ThreadLocal<Random>(() => new Random(Environment.TickCount));

    public const float Pi  = 3.141593f;
    public const float Tau = Pi * 2f;

    public static Random Random => ThreadLocalRandom.Value;

    public static Vector2I P(int x, int y)                       => new Vector2I(x, y);
    public static Vector3I P(int x, int y, int z)                => new Vector3I(x, y, z);
    public static Vector2  V(float x, float y)                   => new Vector2(x, y);
    public static Vector3  V(float x, float y, float z)          => new Vector3(x, y, z);
    public static Vector4  V(float x, float y, float z, float w) => new Vector4(x, y, z, w);

    public static float   NextFloat(this Random random)                       => (float) random.NextDouble();
    public static float   NextFloat(this Random random, float min, float max) => random.NextFloat() * (max - min) + min;
    public static int     NextSign(this Random random)                        => random.Next() < 0.5f ? 1 : -1;
    public static bool    NextBool(this Random random, float chance = 0.5f)   => random.NextDouble() < chance;
    public static bool    NextChance(this Random random, float chance)        => NextBool(random, chance);
    public static int     NextRange(this Random random, IntRange range)       => random.Next(range.Min, range.Max);
    public static float   NextRange(this Random random, FloatRange range)     => random.NextFloat(range.Min, range.Max);
    public static Seed    NextSeed(this Random random)                        => new Seed(random.Next());
    public static Vector2 NextUnitCircle(this Random random)                  => new Vector2(random.NextFloat(-1, 1f), random.NextFloat(-1f, 1f));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DegreesToRadians(float degrees) => (float) (degrees * (Math.PI / 180));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float RadiansToDegrees(float radians) => (float) (radians * (180 / Math.PI));

    public static int   Lerp(int a, int b, float t)     => (int) (a + t * (b - a));
    public static float Lerp(float a, float b, float t) => a + t * (b - a);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(int value, int lower, int upper) {
      if (value < lower) return lower;
      if (value > upper) return upper;

      return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(float value, float lower, float upper) {
      if (value < lower) return lower;
      if (value > upper) return upper;

      return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Wrap(int value, int lower, int upper) {
      if (value < lower) {
        return upper - (lower - value) % (upper - lower);
      }

      return lower + (value - lower) % (upper - lower);
    }
  }
}