using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Surreal.Utilities;

/// <summary>Utilities for working with spans.</summary>
public static class Spans
{
  /// <summary>Converts the given value to a raw span of bytes.</summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Span<byte> AsSpan<T>(ref T value)
    where T : unmanaged
  {
    var span = MemoryMarshal.CreateSpan(ref value, 1);

    return MemoryMarshal.Cast<T, byte>(span);
  }
}
