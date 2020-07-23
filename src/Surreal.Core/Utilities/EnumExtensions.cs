using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Surreal.Mathematics;

namespace Surreal.Utilities {
  public static class EnumExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int AsInt<TEnum>(this TEnum value)
        where TEnum : unmanaged, Enum {
      return *(int*) &value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsFast<TEnum>(this TEnum value, TEnum other)
        where TEnum : unmanaged, Enum {
      return value.AsInt() == other.AsInt();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlagFast<TEnum>(this TEnum value, TEnum comparand)
        where TEnum : unmanaged, Enum {
      var flag = value.AsInt();
      var mask = comparand.AsInt();

      return (flag & mask) == mask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MaskEnumerator<TEnum> GetMaskValues<TEnum>(this TEnum flags)
        where TEnum : unmanaged, Enum {
      return new MaskEnumerator<TEnum>(flags);
    }

    public static TEnum SelectRandomly<TEnum>(this TEnum value, Random random)
        where TEnum : unmanaged, Enum {
      var result     = default(TEnum);
      var enumerator = GetMaskValues(value);

      while (enumerator.MoveNext()) {
        result = enumerator.Current;
        if (random.NextBool()) {
          break;
        }
      }

      return result;
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

    public struct MaskEnumerator<TEnum> : IEnumerator<TEnum>, IEnumerable<TEnum>
        where TEnum : unmanaged, Enum {
      private static readonly TEnum[] Values = CachedEnumLookup<TEnum>.Values;

      private readonly TEnum flags;
      private          int   flag;
      private          int   index;

      public MaskEnumerator(TEnum flags) {
        this.flags = flags;
        flag       = 1;
        index      = -1;
      }

      public TEnum        Current => Values[index];
      object? IEnumerator.Current => Current;

      public bool MoveNext() {
        // tail-recursive, unrolled to loop
        while (true) {
          if (++index >= Values.Length) {
            return false;
          }

          var value = Values[index];
          var mask  = value.AsInt();

          while (flag < mask) {
            flag <<= 1;
          }

          if (flag == mask && flags.HasFlagFast(value)) {
            return true;
          }
        }
      }

      public void Reset() {
        flag  = 1;
        index = -1;
      }

      public void Dispose() {
        // no-op
      }

      public MaskEnumerator<TEnum>          GetEnumerator() => this;
      IEnumerator<TEnum> IEnumerable<TEnum>.GetEnumerator() => GetEnumerator();
      IEnumerator IEnumerable.              GetEnumerator() => GetEnumerator();
    }

    private static class CachedEnumLookup<TEnum>
        where TEnum : unmanaged, Enum {
      public static TEnum[] Values { get; } = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
    }
  }
}