using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Surreal.Collections.Spans {
  public static class SpanExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<TTo> Cast<TFrom, TTo>(this Span<TFrom> span)
        where TFrom : unmanaged
        where TTo : unmanaged {
      return MemoryMarshal.Cast<TFrom, TTo>(span);
    }
  }
}