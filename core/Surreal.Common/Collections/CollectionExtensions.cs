namespace Surreal.Collections;

/// <summary>General purpose collection extensions</summary>
public static class CollectionExtensions
{
  public static Queue<T> ToQueue<T>(this IEnumerable<T> enumerable)
  {
    var result = new Queue<T>();

    foreach (var element in enumerable)
    {
      result.Enqueue(element);
    }

    return result;
  }

  public static T? SelectRandomly<T>(this IEnumerable<T> elements, Random random)
  {
    if (elements is IReadOnlyList<T> list)
    {
      if (list.Count > 0)
      {
        return list[random.Next(0, list.Count)];
      }
    }
    else
    {
      var array = elements.ToArray();
      if (array.Length > 0)
      {
        return array[random.Next(0, array.Length)];
      }
    }

    return default;
  }
}
