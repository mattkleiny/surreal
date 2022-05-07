using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Surreal.Collections;

/// <summary>General purpose collection extensions</summary>
public static class CollectionExtensions
{
  /// <summary>Converts the given <see cref="List{T}"/> to a <see cref="Span{T}"/>.</summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Span<T> AsSpan<T>(this List<T> list)
  {
    return CollectionsMarshal.AsSpan(list);
  }

  /// <summary>Retrieves a value from the dictionary or adds it if it doesn't already exist.</summary>
  public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
    where TKey : notnull
    where TValue : new()
  {
    if (!dictionary.TryGetValue(key, out var value))
    {
      dictionary[key] = value = new TValue();
    }

    return value;
  }

  /// <summary>Selects an item randomly from the list.</summary>
  public static T? SelectRandomly<T>(this IReadOnlyList<T> items, Random random)
  {
    if (items.Count <= 0)
    {
      return default;
    }

    return items[random.Next(0, items.Count)];
  }
}
