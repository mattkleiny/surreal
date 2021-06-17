using System;
using System.Runtime.InteropServices;

namespace Surreal.Collections.Spans {
  public static class SpanExtensions {
    public static Span<TTo> Cast<TFrom, TTo>(this Span<TFrom> span)
        where TFrom : unmanaged
        where TTo : unmanaged {
      return MemoryMarshal.Cast<TFrom, TTo>(span);
    }
  }
}