namespace Surreal.Collections.Slices;

/// <summary>
/// Commonly used extensions for <see cref="Slice{T}" /> and <see cref="ReadOnlySlice{T}" />.
/// </summary>
public static class SliceExtensions
{
  /// <summary>
  /// Converts the given list to a slice.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Slice<T> AsSlice<T>(this List<T> list)
    => list;

  /// <summary>
  /// Converts the given list to a slice with the given offset and length.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Slice<T> AsSlice<T>(this List<T> list, int offset, int length)
    => new(list, offset, length);

  /// <summary>
  /// Converts the given enumerable to a slice.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Slice<T> ToSlice<T>(this IEnumerable<T> enumerable)
    => enumerable.ToList().AsSlice();

  /// <summary>
  /// Converts the given enumerable to a slice with the given offset and length.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Slice<T> ToSlice<T>(this IEnumerable<T> enumerable, int offset, int length)
    => enumerable.ToList().AsSlice(offset, length);

  /// <summary>
  /// Swaps two elements in-place inside the slice.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Swap<T>(this Slice<T> slice, int fromIndex, int toIndex)
    => (slice[fromIndex], slice[toIndex]) = (slice[toIndex], slice[fromIndex]);
}
