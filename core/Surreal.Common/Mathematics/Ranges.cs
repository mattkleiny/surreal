namespace Surreal.Mathematics;

/// <summary>An integral range.</summary>
public readonly record struct IntRange(int Min, int Max)
{
  public int Delta => Max - Min;

  public override string ToString() => $"{Min} to {Max}";
}

/// <summary>A floating-point range.</summary>
public readonly record struct FloatRange(float Min, float Max)
{
  public float Delta => Max - Min;

  public override string ToString() => $"{Min:F} to {Max:F}";
}

/// <summary>A <see cref="TimeSpan"/>  range.</summary>
public readonly record struct TimeSpanRange(TimeSpan Min, TimeSpan Max)
{
  public TimeSpan Delta => Max - Min;

  public override string ToString() => $"{Min:F} to {Max:F}";
}

/// <summary>Utilities for working with ranges.</summary>
public static class Ranges
{
  public static int Clamp(this int value, int min, int max) => value < min ? min : value > max ? max : value;
  public static int Clamp(this int value, IntRange range) => Clamp(value, range.Min, range.Max);
  public static float Clamp(this float value, float min, float max) => value < min ? min : value > max ? max : value;
  public static float Clamp(this float value, FloatRange range) => Clamp(value, range.Min, range.Max);

  public static int ConvertRange(this int value, IntRange oldRange, IntRange newRange)
  {
    if (oldRange.Delta == 0)
    {
      return newRange.Min;
    }

    return (value - oldRange.Min) * newRange.Delta / oldRange.Delta + newRange.Min;
  }

  public static float ConvertRange(this float value, FloatRange oldRange, FloatRange newRange)
  {
    if (Math.Abs(oldRange.Delta) < float.Epsilon)
    {
      return newRange.Min;
    }

    return (value - oldRange.Min) * newRange.Delta / oldRange.Delta + newRange.Min;
  }
}
