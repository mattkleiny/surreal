namespace Surreal.Collections;

/// <summary>Static extensions for dealing with <see cref="IAsyncEnumerable{T}"/> and similar.</summary>
public static class CollectionAsyncExtensions
{
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
