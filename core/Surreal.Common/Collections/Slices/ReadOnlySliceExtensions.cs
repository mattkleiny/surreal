namespace Surreal.Collections.Slices;

/// <summary>
/// Commonly used extensions for <see cref="Slice{T}" /> and <see cref="ReadOnlySlice{T}" />.
/// </summary>
public static class ReadOnlySliceExtensions
{
  /// <summary>
  /// Converts the given list to a read-only slice.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ReadOnlySlice<T> AsReadOnlySlice<T>(this List<T> list)
    => list;

  /// <summary>
  /// Converts the given list to a read-only slice with the given offset and length.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ReadOnlySlice<T> AsReadOnlySlice<T>(this List<T> list, int offset, int length)
    => new(list, offset, length);

  /// <summary>
  /// Converts the given enumerable to a read-only slice.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ReadOnlySlice<T> ToReadOnlySlice<T>(this IEnumerable<T> enumerable)
    => AsReadOnlySlice(enumerable.ToList());

  /// <summary>
  /// Converts the given enumerable to a read-only slice with the given offset and length.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ReadOnlySlice<T> ToReadOnlySlice<T>(this IEnumerable<T> enumerable, int offset, int length)
    => AsReadOnlySlice(enumerable.ToList(), offset, length);
}
