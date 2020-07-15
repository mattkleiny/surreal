using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Surreal.IO {
  public static class SpanExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<TTo> Cast<TFrom, TTo>(this Span<TFrom> span)
        where TFrom : unmanaged
        where TTo : unmanaged {
      return MemoryMarshal.Cast<TFrom, TTo>(span);
    }

    public static ReadOnlySpan<char> Split(this ref ReadOnlySpan<char> span, char seperator) {
      var pos = span.IndexOf(seperator);
      if (pos > -1) {
        var segment = span.Slice(0, pos);
        span = span.Slice(pos + 1);
        return segment;
      } else {
        var segment = span;
        span = span.Slice(span.Length);
        return segment;
      }
    }
  }
}