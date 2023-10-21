namespace Surreal.Collections;

/// <summary>
/// A structured enumeration with support for lookups and complex objects.
/// </summary>
public abstract record Enumeration<T>
  where T : Enumeration<T>
{
  public static ImmutableList<T> All { get; } = DiscoverValues();

  private static ImmutableList<T> DiscoverValues()
  {
    var builder = ImmutableList.CreateBuilder<T>();

    foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static))
    {
      if (property.PropertyType == typeof(T))
      {
        builder.Add((T)property.GetValue(null)!);
      }
    }

    return builder.ToImmutable();
  }
}

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
