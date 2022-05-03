namespace Surreal.Collections;

/// <summary>General purpose collection extensions</summary>
public static class CollectionExtensions
{
  public static T? SelectRandomly<T>(this IReadOnlyList<T> items, Random random)
  {
    if (items.Count > 0)
    {
      return items[random.Next(0, items.Count)];
    }

    return default;
  }
}
