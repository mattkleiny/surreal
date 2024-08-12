namespace Surreal.Collections;

/// <summary>
/// Extensions methods for enums.
/// </summary>
public static class EnumerationExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int AsInt<TEnum>(this TEnum value)
    where TEnum : unmanaged, Enum
  {
    return *(int*)&value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe TEnum AsEnum<TEnum>(this int value)
    where TEnum : unmanaged, Enum
  {
    return *(TEnum*)&value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool EqualsFast<TEnum>(this TEnum first, TEnum second)
    where TEnum : unmanaged, Enum
  {
    var firstInt = first.AsInt();
    var secondInt = second.AsInt();

    return firstInt == secondInt;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool HasFlagFast<TEnum>(this TEnum first, TEnum second)
    where TEnum : unmanaged, Enum
  {
    var flag = first.AsInt();
    var mask = second.AsInt();

    return (flag & mask) == mask;
  }
}
