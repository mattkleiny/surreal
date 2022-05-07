using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Surreal.Collections;

/// <summary>General purpose collection extensions</summary>
public static class CollectionExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Span<T> AsSpan<T>(this List<T> list)
  {
    return CollectionsMarshal.AsSpan(list);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ref TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
    where TKey : notnull
  {
    return ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);
  }

  public static T? SelectRandomly<T>(this IReadOnlyList<T> items, Random random)
  {
    if (items.Count > 0)
    {
      return items[random.Next(0, items.Count)];
    }

    return default;
  }
}
