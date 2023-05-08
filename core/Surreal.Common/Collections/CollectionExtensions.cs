using System.Runtime.CompilerServices;
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
  {
    return CollectionsMarshal.AsSpan(list);
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
