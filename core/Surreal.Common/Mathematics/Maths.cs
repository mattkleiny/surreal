using Surreal.Collections;

namespace Surreal.Mathematics;

/// <summary>Common used mathematical utilities.</summary>
public static class Maths
{
  public static float DegreesToRadians(float degrees) => (float) (degrees * (Math.PI / 180));
  public static float RadiansToDegrees(float radians) => (float) (radians * (180 / Math.PI));

  public static int Lerp(int a, int b, float t) => (int) (a + t * (b - a));
  public static float Lerp(float a, float b, float t) => a + t * (b - a);

  public static float PingPong(float t) => (MathF.Sin(t) + 1f) / 2f;

  public static int Wrap(int value, int lower, int upper)
  {
    if (value < lower)
    {
      return upper - (lower - value) % (upper - lower);
    }

    return lower + (value - lower) % (upper - lower);
  }
}

/// <summary>Extensions for <see cref="Random"/>.</summary>
public static class RandomExtensions
{
  public static int NextInt(this Random random)
    => random.Next();

  public static float NextFloat(this Random random)
    => (float) random.NextDouble();

  public static int NextInt(this Random random, int min, int max)
    => random.Next(min, max + 1);

  public static float NextFloat(this Random random, float min, float max)
    => random.NextFloat() * (max - min) + min;

  public static double NextDouble(this Random random, double min, double max)
    => random.NextDouble() * (max - min) + min;

  public static int NextSign(this Random random)
    => random.Next() < 0.5f ? 1 : -1;

  public static bool NextBool(this Random random, float chance = 0.5f)
    => random.NextDouble() < chance;

  public static bool NextChance(this Random random, float chance)
    => NextBool(random, chance);

  public static TimeSpan NextTimeSpan(this Random random)
    => TimeSpan.FromSeconds(random.NextDouble());

  public static int NextRange(this Random random, IntRange range)
    => random.Next(range.Min, range.Max);

  public static float NextRange(this Random random, FloatRange range)
    => random.NextFloat(range.Min, range.Max);

  public static TimeSpan NextRange(this Random random, TimeSpanRange range)
    => TimeSpan.FromSeconds(random.NextDouble(range.Min.TotalSeconds, range.Max.TotalSeconds));

  public static Seed NextSeed(this Random random)
    => new(random.Next());

  public static Color NextColor(this Random random)
    => new(random.NextFloat(), random.NextFloat(), random.NextFloat());

  public static Color32 NextColor32(this Random random)
    => new((byte) random.NextInt(0, 255), (byte) random.NextInt(0, 255), (byte) random.NextInt(0, 255));

  public static Point2 NextPoint2(this Random random, int min, int max)
    => new(random.NextInt(min, max), random.NextInt(min, max));

  public static Point3 NextPoint3(this Random random, int min, int max)
    => new(random.NextInt(min, max), random.NextInt(min, max), random.NextInt(min, max));

  public static Vector2 NextVector2(this Random random, float min, float max)
    => new(random.NextFloat(min, max), random.NextFloat(min, max));

  public static Vector3 NextVector3(this Random random, float min, float max)
    => new(random.NextFloat(min, max), random.NextFloat(min, max), random.NextFloat(min, max));

  public static Vector4 NextVector4(this Random random, float min, float max)
    => new(random.NextFloat(min, max), random.NextFloat(min, max), random.NextFloat(min, max), random.NextFloat(min, max));

  public static Vector2 NextUnitCircle(this Random random)
    => new(random.NextFloat(-1, 1f), random.NextFloat(-1f, 1f));

  public static Vector3 NextUnitSphere(this Random random)
    => new(random.NextFloat(-1, 1f), random.NextFloat(-1f, 1f), random.NextFloat(-1f, 1f));

  public static TEnum NextEnum<TEnum>(this Random random)
    where TEnum : unmanaged, Enum
  {
    var values = EnumExtensions.GetEnumValues<TEnum>();

    var min = values[0].AsInt();
    var max = values[^1].AsInt();

    return random.Next(min, max).AsEnum<TEnum>();
  }

  public static TEnum NextEnumMask<TEnum>(this Random random, TEnum mask)
    where TEnum : unmanaged, Enum
  {
    var result = default(TEnum);
    var enumerator = mask.GetMaskValues();

    while (enumerator.MoveNext())
    {
      result = enumerator.Current;

      if (result.AsInt() != 0 && random.NextBool())
      {
        break;
      }
    }

    return result;
  }
}
