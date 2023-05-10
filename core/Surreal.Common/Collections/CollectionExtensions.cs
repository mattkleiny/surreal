using System.Runtime.InteropServices;

namespace Surreal.Collections;

/// <summary>
/// General purpose collection extensions
/// </summary>
public static class CollectionExtensions
{
  /// <summary>
  /// Converts the given <see cref="List{T}" /> to a <see cref="Span{T}" />.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Span<T> AsSpan<T>(this List<T> list) => CollectionsMarshal.AsSpan(list);

  /// <summary>
  /// Converts the given <see cref="List{T}" /> to a <see cref="Span{T}" /> with the given offset and length.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Span<T> AsSpan<T>(this List<T> list, int offset, int length) => CollectionsMarshal.AsSpan(list).Slice(offset, length);

  /// <summary>
  /// Converts the given <see cref="List{T}" /> to a <see cref="ReadOnlySpan{T}" />.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ReadOnlySpan<T> AsReadOnlySpan<T>(this List<T> list) => CollectionsMarshal.AsSpan(list);

  /// <summary>
  /// Converts the given <see cref="List{T}" /> to a <see cref="ReadOnlySpan{T}" /> with the given offset and length.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ReadOnlySpan<T> AsReadOnlySpan<T>(this List<T> list, int offset, int length) => CollectionsMarshal.AsSpan(list).Slice(offset, length);

  /// <summary>
  /// Swaps two elements in-place inside the list.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Swap<T>(this List<T> list, int fromIndex, int toIndex)
  {
    (list[fromIndex], list[toIndex]) = (list[toIndex], list[fromIndex]);
  }

  /// <summary>
  /// Retrieves a reference to a value type in a dictionary or creates it if it doesn't already exist.
  /// </summary>
  [SuppressMessage("ReSharper", "CanSimplifyDictionaryLookupWithTryAdd")]
  public static ref TValue GetOrCreateRef<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
    where TKey : notnull
    where TValue : new()
  {
    if (!dictionary.ContainsKey(key))
    {
      dictionary.Add(key, new TValue());
    }

    return ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);
  }
}
