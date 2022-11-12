using System.Runtime.CompilerServices;
using Surreal.Text;

namespace Surreal.Collections;

/// <summary>Extensions methods for enums.</summary>
public static class EnumExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe int AsInt<TEnum>(this TEnum value)
    where TEnum : unmanaged, Enum
  {
    return *(int*) &value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static unsafe TEnum AsEnum<TEnum>(this int value)
    where TEnum : unmanaged, Enum
  {
    return *(TEnum*) &value;
  }

  public static bool EqualsFast<TEnum>(this TEnum first, TEnum second)
    where TEnum : unmanaged, Enum
  {
    var firstInt = first.AsInt();
    var secondInt = second.AsInt();

    return firstInt == secondInt;
  }

  public static bool HasFlagFast<TEnum>(this TEnum first, TEnum second)
    where TEnum : unmanaged, Enum
  {
    var flag = first.AsInt();
    var mask = second.AsInt();

    return (flag & mask) == mask;
  }

  public static ImmutableArray<string> GetEnumNames<TEnum>()
    where TEnum : unmanaged, Enum
  {
    return CachedEnumLookup<TEnum>.Names;
  }

  public static ImmutableArray<TEnum> GetEnumValues<TEnum>()
    where TEnum : unmanaged, Enum
  {
    return CachedEnumLookup<TEnum>.Values;
  }

  public static MaskEnumerator<TEnum> GetMaskValues<TEnum>(this TEnum flags)
    where TEnum : unmanaged, Enum
  {
    return new MaskEnumerator<TEnum>(flags);
  }

  public static string ToStringFast<TEnum>(this TEnum value)
    where TEnum : unmanaged, Enum
  {
    return CachedEnumLookup<TEnum>.Names[value.AsInt()];
  }

  public static string ToPermutationString<TEnum>(this TEnum flags)
    where TEnum : unmanaged, Enum
  {
    var builder = new StringBuilder();

    foreach (var flag in flags.GetMaskValues()) builder.AppendWithSeparator(flag.ToString(), " | ");

    if (builder.Length == 0)
    {
      builder.Append("None");
    }

    return builder.ToString();
  }

  public struct MaskEnumerator<TEnum> : IEnumerator<TEnum>, IEnumerable<TEnum>
    where TEnum : unmanaged, Enum
  {
    private readonly TEnum _flags;
    private int _flag;
    private int _index;

    public MaskEnumerator(TEnum flags)
    {
      _flags = flags;
      _flag = 1;
      _index = -1;
    }

    public TEnum Current => CachedEnumLookup<TEnum>.Values[_index];
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      // tail-recursive, unrolled to loop
      while (true)
      {
        var values = CachedEnumLookup<TEnum>.Values;
        if (++_index >= values.Length)
        {
          return false;
        }

        var value = values[_index];
        var mask = value.AsInt();

        while (_flag < mask) _flag <<= 1;

        if (_flag == mask && _flags.HasFlagFast(value))
        {
          return true;
        }
      }
    }

    public void Reset()
    {
      _flag = 1;
      _index = -1;
    }

    public void Dispose()
    {
      // no-op
    }

    public MaskEnumerator<TEnum> GetEnumerator()
    {
      return this;
    }

    IEnumerator<TEnum> IEnumerable<TEnum>.GetEnumerator()
    {
      return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }

  private static class CachedEnumLookup<TEnum>
    where TEnum : unmanaged, Enum
  {
    public static ImmutableArray<string> Names { get; } = Enum.GetNames(typeof(TEnum)).ToImmutableArray();
    public static ImmutableArray<TEnum> Values { get; } = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToImmutableArray();
  }
}

