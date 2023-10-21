namespace Surreal.Maths;

/// <summary>
/// Allows for generating a value from a <see cref="Random"/> instance.
/// </summary>
public interface IFromRandom<out TSelf>
{
  /// <summary>
  /// Creates a new instance from a <see cref="Random"/> instance.
  /// </summary>
  static abstract TSelf FromRandom(Random random);
}

/// <summary>
/// Extensions for <see cref="Random" />.
/// </summary>
public static class RandomExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static byte NextByte(this Random random)
    => (byte)(random.Next() % 255);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int NextInt(this Random random)
    => random.Next();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int NextInt(this Random random, int min, int max)
    => random.Next(min, max + 1);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float NextFloat(this Random random)
    => (float)random.NextDouble();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float NextFloat(this Random random, float min, float max)
    => random.NextFloat() * (max - min) + min;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static double NextDouble(this Random random, double min, double max)
    => random.NextDouble() * (max - min) + min;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int NextSign(this Random random)
    => random.Next() < 0.5f ? 1 : -1;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool NextBool(this Random random, float chance = 0.5f)
    => random.NextSingle() < chance;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool NextChance(this Random random, float chance)
    => NextBool(random, chance);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int NextRange(this Random random, Range<int> range)
    => random.Next(range.Min, range.Max);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float NextRange(this Random random, Range<float> range)
    => random.NextFloat(range.Min, range.Max);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static double NextRange(this Random random, Range<double> range)
    => random.NextDouble(range.Min, range.Max);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Point2 NextPoint2(this Random random, int min, int max)
    => new(random.NextInt(min, max), random.NextInt(min, max));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Point3 NextPoint3(this Random random, int min, int max)
    => new(random.NextInt(min, max), random.NextInt(min, max), random.NextInt(min, max));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector2 NextVector2(this Random random, float min, float max)
    => new(random.NextFloat(min, max), random.NextFloat(min, max));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector3 NextVector3(this Random random, float min, float max)
    => new(random.NextFloat(min, max), random.NextFloat(min, max), random.NextFloat(min, max));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector2 NextUnitCircle(this Random random)
    => new(random.NextFloat(-1, 1f), random.NextFloat(-1f, 1f));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector3 NextUnitSphere(this Random random)
    => new(random.NextFloat(-1, 1f), random.NextFloat(-1f, 1f), random.NextFloat(-1f, 1f));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Next<T>(this Random random)
    where T : IFromRandom<T> => T.FromRandom(random);
}
