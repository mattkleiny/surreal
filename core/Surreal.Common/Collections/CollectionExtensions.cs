using System.Diagnostics.CodeAnalysis;

namespace Surreal.Collections;

/// <summary>General purpose collection extensions</summary>
public static class CollectionExtensions
{
  public static T? SelectRandomly<T>(this IEnumerable<T> elements, Random random)
  {
    switch (elements)
    {
      case IReadOnlyList<T> list:
      {
        if (list.Count > 0)
        {
          return list[random.Next(0, list.Count)];
        }

        break;
      }
      default:
      {
        var array = elements.ToArray();
        if (array.Length > 0)
        {
          return array[random.Next(0, array.Length)];
        }

        break;
      }
    }

    return default;
  }

  public static bool TryPeekAndDequeue<T>(this Queue<T> queue, Predicate<T> predicate, [NotNullWhen(true)] out T? result)
    where T : notnull
  {
    if (queue.TryPeek(out var value))
    {
      if (predicate(value))
      {
        result = queue.Dequeue();
        return true;
      }
    }

    result = default;
    return false;
  }
}
