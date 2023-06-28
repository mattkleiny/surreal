using Surreal.Colors;

namespace Surreal.Mathematics;

/// <summary>
/// Extensions for <see cref="Random" />.
/// </summary>
public static class RandomExtensions
{
  public static int NextInt(this Random random)
  {
    return random.Next();
  }

  public static int NextInt(this Random random, int min, int max)
  {
    return random.Next(min, max + 1);
  }

  public static float NextFloat(this Random random)
  {
    return (float)random.NextDouble();
  }

  public static float NextFloat(this Random random, float min, float max)
  {
    return random.NextFloat() * (max - min) + min;
  }

  public static double NextDouble(this Random random, double min, double max)
  {
    return random.NextDouble() * (max - min) + min;
  }

  public static int NextSign(this Random random)
  {
    return random.Next() < 0.5f ? 1 : -1;
  }

  public static bool NextBool(this Random random, float chance = 0.5f)
  {
    return random.NextSingle() < chance;
  }

  public static bool NextChance(this Random random, float chance)
  {
    return NextBool(random, chance);
  }

  public static int NextRange(this Random random, IntRange range)
  {
    return random.Next(range.Min, range.Max);
  }

  public static float NextRange(this Random random, FloatRange range)
  {
    return random.NextFloat(range.Min, range.Max);
  }

  public static Seed NextSeed(this Random random)
  {
    return new Seed(random.Next());
  }

  public static Color NextColorF(this Random random)
  {
    return new Color(random.NextFloat(), random.NextFloat(), random.NextFloat());
  }

  public static Color32 NextColorB(this Random random)
  {
    return new Color32((byte)random.NextInt(0, 255), (byte)random.NextInt(0, 255), (byte)random.NextInt(0, 255));
  }

  public static Point2 NextPoint2(this Random random, int min, int max)
  {
    return new Point2(random.NextInt(min, max), random.NextInt(min, max));
  }

  public static Point3 NextPoint3(this Random random, int min, int max)
  {
    return new Point3(random.NextInt(min, max), random.NextInt(min, max), random.NextInt(min, max));
  }

  public static Point4 NextPoint4(this Random random, int min, int max)
  {
    return new Point4(random.NextInt(min, max), random.NextInt(min, max), random.NextInt(min, max), random.NextInt(min, max));
  }

  public static Vector2 NextVector2(this Random random, float min, float max)
  {
    return new Vector2(random.NextFloat(min, max), random.NextFloat(min, max));
  }

  public static Vector3 NextVector3(this Random random, float min, float max)
  {
    return new Vector3(random.NextFloat(min, max), random.NextFloat(min, max), random.NextFloat(min, max));
  }

  public static Vector4 NextVector4(this Random random, float min, float max)
  {
    return new Vector4(random.NextFloat(min, max), random.NextFloat(min, max), random.NextFloat(min, max), random.NextFloat(min, max));
  }

  public static Vector2 NextUnitCircle(this Random random)
  {
    return new Vector2(random.NextFloat(-1, 1f), random.NextFloat(-1f, 1f));
  }

  public static Vector3 NextUnitSphere(this Random random)
  {
    return new Vector3(random.NextFloat(-1, 1f), random.NextFloat(-1f, 1f), random.NextFloat(-1f, 1f));
  }
}
