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
  public static Span<T> AsSpan<T>(this List<T> list)
    => CollectionsMarshal.AsSpan(list);

  /// <summary>
  /// Converts the given <see cref="List{T}" /> to a <see cref="Span{T}" /> with the given offset and length.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Span<T> AsSpan<T>(this List<T> list, int offset, int length)
    => CollectionsMarshal.AsSpan(list).Slice(offset, length);

  /// <summary>
  /// Converts the given <see cref="List{T}" /> to a <see cref="ReadOnlySpan{T}" />.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ReadOnlySpan<T> AsReadOnlySpan<T>(this List<T> list)
    => CollectionsMarshal.AsSpan(list);

  /// <summary>
  /// Converts the given <see cref="List{T}" /> to a <see cref="ReadOnlySpan{T}" /> with the given offset and length.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ReadOnlySpan<T> AsReadOnlySpan<T>(this List<T> list, int offset, int length)
    => CollectionsMarshal.AsSpan(list).Slice(offset, length);

  /// <summary>
  /// Swaps two elements in-place inside the array.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Swap<T>(this T[] array, int fromIndex, int toIndex)
  {
    (array[fromIndex], array[toIndex]) = (array[toIndex], array[fromIndex]);
  }

  /// <summary>
  /// Swaps two elements in-place inside the list.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Swap<T>(this List<T> list, int fromIndex, int toIndex)
  {
    (list[fromIndex], list[toIndex]) = (list[toIndex], list[fromIndex]);
  }

  /// <summary>
  /// Selects a random element from the given list.
  /// </summary>
  public static T SelectRandom<T>(this IReadOnlyList<T> list)
  {
    return SelectRandom(list, Random.Shared);
  }

  /// <summary>
  /// Selects a random element from the given list.
  /// </summary>
  public static T SelectRandom<T>(this IReadOnlyList<T> list, Random random)
  {
    if (list.Count == 0)
    {
      throw new InvalidOperationException("Cannot select a random element from an empty list.");
    }

    return list[random.Next(0, list.Count)];
  }

  /// <summary>
  /// Selects a random element from the given slice.
  /// </summary>
  public static T SelectRandom<T>(this ReadOnlySlice<T> slice)
  {
    return SelectRandom(slice, Random.Shared);
  }

  /// <summary>
  /// Selects a random element from the given slice.
  /// </summary>
  public static T SelectRandom<T>(this ReadOnlySlice<T> slice, Random random)
  {
    if (slice.Length == 0)
    {
      throw new InvalidOperationException("Cannot select a random element from an empty slice.");
    }

    return slice[random.Next(0, slice.Length)];
  }

  /// <summary>
  /// Selects a random element from the given list.
  /// </summary>
  public static T SelectRandom<T>(this ReadOnlySpan<T> slice)
  {
    return SelectRandom(slice, Random.Shared);
  }

  /// <summary>
  /// Selects a random element from the given list.
  /// </summary>
  public static T SelectRandom<T>(this ReadOnlySpan<T> slice, Random random)
  {
    if (slice.Length == 0)
    {
      throw new InvalidOperationException("Cannot select a random element from an empty slice.");
    }

    return slice[random.Next(0, slice.Length)];
  }

  /// <summary>
  /// Selects a random element from the given <see cref="IEnumerable{T}"/>.
  /// </summary>
  public static T SelectRandom<T>(this IEnumerable<T> enumerable)
  {
    return SelectRandom(enumerable, Random.Shared);
  }

  /// <summary>
  /// Selects a random element from the given <see cref="IEnumerable{T}"/>.
  /// </summary>
  public static T SelectRandom<T>(this IEnumerable<T> enumerable, Random random)
  {
    return SelectRandom(enumerable.ToList(), random);
  }

  /// <summary>
  /// Selects a random element from the given list.
  /// </summary>
  public static bool TrySelectRandom<T>(this IReadOnlyList<T> list, [MaybeNullWhen(false)] out T result)
  {
    return TrySelectRandom(list, Random.Shared, out result);
  }

  /// <summary>
  /// Selects a random element from the given list.
  /// </summary>
  public static bool TrySelectRandom<T>(this IReadOnlyList<T> list, Random random, [MaybeNullWhen(false)] out T result)
  {
    if (list.Count == 0)
    {
      result = default;
      return false;
    }

    result = list[random.Next(0, list.Count)];
    return true;
  }

  /// <summary>
  /// Selects a random element from the given slice.
  /// </summary>
  public static bool TrySelectRandom<T>(this ReadOnlySlice<T> slice, [MaybeNullWhen(false)] out T result)
  {
    return TrySelectRandom(slice, Random.Shared, out result);
  }

  /// <summary>
  /// Selects a random element from the given slice.
  /// </summary>
  public static bool TrySelectRandom<T>(this ReadOnlySlice<T> slice, Random random, [MaybeNullWhen(false)] out T result)
  {
    if (slice.Length == 0)
    {
      result = default;
      return false;
    }

    result = slice[random.Next(0, slice.Length)];
    return true;
  }

  /// <summary>
  /// Selects a random element from the given slice.
  /// </summary>
  public static bool TrySelectRandom<T>(this ReadOnlySpan<T> span, [MaybeNullWhen(false)] out T result)
  {
    return TrySelectRandom(span, Random.Shared, out result);
  }

  /// <summary>
  /// Selects a random element from the given span.
  /// </summary>
  public static bool TrySelectRandom<T>(this ReadOnlySpan<T> span, Random random, [MaybeNullWhen(false)] out T result)
  {
    if (span.Length == 0)
    {
      result = default;
      return false;
    }

    result = span[random.Next(0, span.Length)];
    return true;
  }

  /// <summary>
  /// Selects a random element from the given <see cref="IEnumerable{T}"/>.
  /// </summary>
  public static bool TrySelectRandom<T>(this IEnumerable<T> enumerable, [MaybeNullWhen(false)] out T result)
  {
    return TrySelectRandom(enumerable, Random.Shared, out result);
  }

  /// <summary>
  /// Selects a random element from the given <see cref="IEnumerable{T}"/>.
  /// </summary>
  public static bool TrySelectRandom<T>(this IEnumerable<T> enumerable, Random random, [MaybeNullWhen(false)] out T result)
  {
    return TrySelectRandom(enumerable.ToReadOnlySlice(), random, out result);
  }

  /// <summary>
  /// Shuffles the given <see cref="IEnumerable{T}"/> using the Fisher-Yates algorithm.
  /// </summary>
  public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
  {
    return Shuffle(source, Random.Shared);
  }

  /// <summary>
  /// Shuffles the given <see cref="IEnumerable{T}"/> using the Fisher-Yates algorithm.
  /// </summary>
  public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random random)
  {
    var elements = source.ToArray();

    for (var fromIndex = elements.Length - 1; fromIndex > 0; fromIndex--)
    {
      var swapIndex = random.Next(fromIndex + 1);

      elements.Swap(fromIndex, swapIndex);
    }

    return elements;
  }
}
