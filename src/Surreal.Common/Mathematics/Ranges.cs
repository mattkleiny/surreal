using System;

namespace Surreal.Mathematics
{
  public static class Range
  {
    public static int   Clamp(this int value, int min, int max)       => Maths.Clamp(value, min, max);
    public static float Clamp(this float value, float min, float max) => Maths.Clamp(value, min, max);
    public static int   Clamp(this int value, IntRange range)         => Maths.Clamp(value, range.Min, range.Max);
    public static float Clamp(this float value, FloatRange range)     => Maths.Clamp(value, range.Min, range.Max);

    public static bool Between(this int value, int min, int max)       => value > min && value < max;
    public static bool Between(this float value, float min, float max) => value > min && value < max;
    public static bool Between(this int value, IntRange range)         => value > range.Min && value < range.Max;
    public static bool Between(this float value, FloatRange range)     => value > range.Min && value < range.Max;

    public static int ConvertRange(this int value, IntRange oldRange, IntRange newRange)
    {
      if (oldRange.Delta == 0) return newRange.Min;

      return (value - oldRange.Min) * newRange.Delta / oldRange.Delta + newRange.Min;
    }

    public static float ConvertRange(this float value, FloatRange oldRange, FloatRange newRange)
    {
      if (Math.Abs(oldRange.Delta) < float.Epsilon) return newRange.Min;

      return (value - oldRange.Min) * newRange.Delta / oldRange.Delta + newRange.Min;
    }
  }

  public readonly record struct IntSize(int Width, int Height)
  {
    public override string ToString() => $"{Width} x {Height}";
  }

  public readonly record struct FloatSize(int Width, int Height)
  {
    public override string ToString() => $"{Width} x {Height}";
  }

  public readonly record struct IntRange(int Min, int Max)
  {
    public int Delta => Max - Min;

    public override string ToString() => $"{Min} to {Max}";
  }

  public readonly record struct FloatRange(float Min, float Max)
  {
    public float Delta => Max - Min;

    public override string ToString() => $"{Min:F} to {Max:F}";
  }
}
