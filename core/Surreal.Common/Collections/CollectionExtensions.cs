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

  public static async IAsyncEnumerable<TResult> SelectAsync<T, TResult>(this IEnumerable<T> enumerable, Func<T, ValueTask<TResult>> selector)
  {
    foreach (var element in enumerable)
    {
      yield return await selector(element);
    }
  }

  public static async ValueTask<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> enumerable, CancellationToken cancellationToken = default)
  {
    var results = new List<T>();

    await foreach (var element in enumerable.WithCancellation(cancellationToken))
    {
      results.Add(element);
    }

    return results;
  }
}
