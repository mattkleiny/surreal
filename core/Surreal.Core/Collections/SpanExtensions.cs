using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Surreal.Collections {
  public static class SpanExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<TTo> Cast<TFrom, TTo>(this Span<TFrom> span)
        where TFrom : unmanaged
        where TTo : unmanaged {
      return MemoryMarshal.Cast<TFrom, TTo>(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap<T>(this Span<T> span, int from, int to) {
      var temp = span[from];
      span[from] = span[to];
      span[to]   = temp;
    }

    public static void Reverse<T>(this Span<T> span)
        where T : IComparable<T> {
      for (var i = 0; i < span.Length / 2; i++) {
        span.Swap(i, span.Length - i - 1);
      }
    }
  }
}