using System;

namespace Surreal.Mathematics
{
  public static class Range
  {
    public static IntRange   Of(int min, int max)     => new(min, max);
    public static FloatRange Of(float min, float max) => new(min, max);

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

  public readonly struct IntRange : IEquatable<IntRange>
  {
    public readonly int Min;
    public readonly int Max;

    public IntRange(int min, int max)
    {
      Min = min;
      Max = max;
    }

    public void Deconstruct(out int min, out int max)
    {
      min = Min;
      max = Max;
    }

    public int Delta => Max - Min;

    public override string ToString() => $"{Min.ToString()} to {Max.ToString()}";

    public          bool Equals(IntRange other) => Min == other.Min && Max == other.Max;
    public override bool Equals(object? obj)    => obj is IntRange other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Min, Max);

    public static bool operator ==(IntRange left, IntRange right) => left.Equals(right);
    public static bool operator !=(IntRange left, IntRange right) => !left.Equals(right);
  }

  public readonly struct FloatRange : IEquatable<FloatRange>
  {
    public readonly float Min;
    public readonly float Max;

    public FloatRange(float min, float max)
    {
      Min = min;
      Max = max;
    }

    public void Deconstruct(out float min, out float max)
    {
      min = Min;
      max = Max;
    }

    public float Delta => Max - Min;

    public override string ToString() => $"{Min.ToString("F")} to {Max.ToString("F")}";

    public bool Equals(FloatRange other) =>
        Math.Abs(Min - other.Min) < float.Epsilon &&
        Math.Abs(Max - other.Max) < float.Epsilon;

    public override bool Equals(object? obj) => obj is FloatRange other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Min, Max);

    public static bool operator ==(FloatRange left, FloatRange right) => left.Equals(right);
    public static bool operator !=(FloatRange left, FloatRange right) => !left.Equals(right);
  }
}