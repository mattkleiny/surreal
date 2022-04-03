using System.Runtime.CompilerServices;
using Surreal.Mathematics;
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
    var firstInt  = first.AsInt();
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

  public static TEnum SelectMaskRandomly<TEnum>(this TEnum value, Random random)
    where TEnum : unmanaged, Enum
  {
    var result     = default(TEnum);
    var enumerator = GetMaskValues(value);

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

  public static string ToStringFast<TEnum>(this TEnum value)
    where TEnum : unmanaged, Enum
  {
    return CachedEnumLookup<TEnum>.Names[value.AsInt()];
  }

  public static string ToPermutationString<TEnum>(this TEnum flags)
    where TEnum : unmanaged, Enum
  {
    var builder = new StringBuilder();

    foreach (var flag in flags.GetMaskValues())
    {
      builder.AppendWithSeparator(flag.ToString(), " | ");
    }

    if (builder.Length == 0)
    {
      builder.Append("None");
    }

    return builder.ToString();
  }

  public struct MaskEnumerator<TEnum> : IEnumerator<TEnum>, IEnumerable<TEnum>
    where TEnum : unmanaged, Enum
  {
    private readonly TEnum flags;
    private          int   flag;
    private          int   index;

    public MaskEnumerator(TEnum flags)
    {
      this.flags = flags;
      flag       = 1;
      index      = -1;
    }

    public TEnum       Current => CachedEnumLookup<TEnum>.Values[index];
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      // tail-recursive, unrolled to loop
      while (true)
      {
        var values = CachedEnumLookup<TEnum>.Values;
        if (++index >= values.Length)
        {
          return false;
        }

        var value = values[index];
        var mask  = value.AsInt();

        while (flag < mask)
        {
          flag <<= 1;
        }

        if (flag == mask && flags.HasFlagFast(value))
        {
          return true;
        }
      }
    }

    public void Reset()
    {
      flag  = 1;
      index = -1;
    }

    public void Dispose()
    {
      // no-op
    }

    public MaskEnumerator<TEnum>          GetEnumerator() => this;
    IEnumerator<TEnum> IEnumerable<TEnum>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.              GetEnumerator() => GetEnumerator();
  }

  private static class CachedEnumLookup<TEnum>
    where TEnum : unmanaged, Enum
  {
    public static ImmutableArray<string> Names  { get; } = Enum.GetNames(typeof(TEnum)).ToImmutableArray();
    public static ImmutableArray<TEnum>  Values { get; } = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToImmutableArray();
  }
}
