using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Surreal.Collections;

namespace Surreal {
  public static class EnumExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlagFast<TEnum>(this TEnum value, TEnum comparand)
        where TEnum : unmanaged, Enum {
      var flag = value.AsInt();
      var mask = comparand.AsInt();

      return (flag & mask) == mask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int AsInt<TEnum>(this TEnum value)
        where TEnum : unmanaged, Enum {
      return *(int*) &value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum SelectRandomly<TEnum>(this TEnum value, Random random)
        where TEnum : unmanaged, Enum {
      return GetMaskValues(value).SelectRandomly(random);
    }

    public static List<TEnum> GetMaskValues<TEnum>(this TEnum flags)
        where TEnum : unmanaged, Enum {
      var flag    = 1;
      var results = new List<TEnum>();

      foreach (var value in CachedEnumLookup<TEnum>.Values) {
        var mask = value.AsInt();

        while (flag < mask) {
          flag <<= 1;
        }

        if (flag == mask && flags.HasFlagFast(value)) {
          results.Add(value);
        }
      }

      return results;
    }

    public static string ToPermutationString<TEnum>(this TEnum flags)
        where TEnum : unmanaged, Enum {
      const string seperator = " | ";

      var builder = new StringBuilder();

      foreach (var flag in flags.GetMaskValues()) {
        builder.AppendWithSeparator(flag.ToString(), seperator);
      }

      if (builder.Length == 0) {
        builder.Append("None");
      }

      return builder.ToString();
    }

    private static class CachedEnumLookup<TEnum>
        where TEnum : unmanaged, Enum {
      public static string[] Names  { get; } = Enum.GetNames(typeof(TEnum)).ToArray();
      public static TEnum[]  Values { get; } = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
    }
  }
}