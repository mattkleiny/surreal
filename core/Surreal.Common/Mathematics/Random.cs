using Surreal.Collections;

namespace Surreal.Mathematics;

/// <summary>Extensions for <see cref="Random"/>.</summary>
public static class RandomExtensions
{
  public static int NextInt(this Random random) => random.Next();
  public static int NextInt(this Random random, int min, int max) => random.Next(min, max + 1);

  public static float NextFloat(this Random random) => (float)random.NextDouble();
  public static float NextFloat(this Random random, float min, float max) => random.NextFloat() * (max - min) + min;

  public static double NextDouble(this Random random, double min, double max) => random.NextDouble() * (max - min) + min;

  public static int NextSign(this Random random) => random.Next() < 0.5f ? 1 : -1;
  public static bool NextBool(this Random random, float chance = 0.5f) => random.NextChance(chance);
  public static bool NextChance(this Random random, float chance) => NextBool(random, chance);

  public static int NextRange(this Random random, IntRange range) => random.Next(range.Min, range.Max);
  public static float NextRange(this Random random, FloatRange range) => random.NextFloat(range.Min, range.Max);

  public static Seed NextSeed(this Random random) => new(random.Next());

  public static Color NextColor(this Random random) => new(random.NextFloat(), random.NextFloat(), random.NextFloat());
  public static Color32 NextColor32(this Random random) => new((byte)random.NextInt(0, 255), (byte)random.NextInt(0, 255), (byte)random.NextInt(0, 255));

  public static Point2 NextPoint2(this Random random, int min, int max) => new(random.NextInt(min, max), random.NextInt(min, max));
  public static Point3 NextPoint3(this Random random, int min, int max) => new(random.NextInt(min, max), random.NextInt(min, max), random.NextInt(min, max));
  public static Point4 NextPoint4(this Random random, int min, int max) => new(random.NextInt(min, max), random.NextInt(min, max), random.NextInt(min, max), random.NextInt(min, max));

  public static Vector2 NextVector2(this Random random, float min, float max) => new(random.NextFloat(min, max), random.NextFloat(min, max));
  public static Vector3 NextVector3(this Random random, float min, float max) => new(random.NextFloat(min, max), random.NextFloat(min, max), random.NextFloat(min, max));
  public static Vector4 NextVector4(this Random random, float min, float max) => new(random.NextFloat(min, max), random.NextFloat(min, max), random.NextFloat(min, max), random.NextFloat(min, max));

  public static Vector2 NextUnitCircle(this Random random) => new(random.NextFloat(-1, 1f), random.NextFloat(-1f, 1f));
  public static Vector3 NextUnitSphere(this Random random) => new(random.NextFloat(-1, 1f), random.NextFloat(-1f, 1f), random.NextFloat(-1f, 1f));

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
