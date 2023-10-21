namespace Surreal.Maths;

/// <summary>
/// A range of <see cref="T"/>.
/// </summary>
public readonly record struct Range<T>(T Min, T Max)
  where T : INumber<T>
{
  public T Delta => Max - Min;

  public override string ToString()
  {
    return $"{Min} to {Max}";
  }
}

/// <summary>
/// Utilities for working with <see cref="Range{T}"/>s.
/// </summary>
public static class Ranges
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Clamp<T>(this T value, T min, T max)
    where T : INumber<T> => T.Clamp(value, min, max);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Clamp<T>(this T value, Range<T> range)
    where T : INumber<T> => T.Clamp(value, range.Min, range.Max);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T ConvertRange<T>(this T value, Range<T> oldRange, Range<T> newRange)
    where T : INumber<T>
  {
    if (oldRange.Delta == T.Zero)
    {
      return newRange.Min;
    }

    return (value - oldRange.Min) * newRange.Delta / oldRange.Delta + newRange.Min;
  }
}
