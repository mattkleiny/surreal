using System.Collections.Generic;

namespace Surreal.Collections
{
  public static class CollectionExtensions
  {
    public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull
        where TValue : new()
    {
      if (!dictionary.TryGetValue(key, out var value))
      {
        dictionary[key] = value = new TValue();
      }

      return value;
    }

    public static void Swap<T>(this IList<T?> array, int fromIndex, int toIndex)
    {
      var temp = array[fromIndex];

      array[fromIndex] = array[toIndex];
      array[toIndex]   = temp;
    }

    public static bool TryPeek<T>(this Stack<T> stack, out T value)
    {
      if (stack.Count > 0)
      {
        value = stack.Peek();
        return true;
      }

      value = default!;
      return false;
    }

    public static bool TryPop<T>(this Stack<T> stack, out T value)
    {
      if (stack.Count > 0)
      {
        value = stack.Pop();
        return true;
      }

      value = default!;
      return false;
    }

    public static bool TryPeek<T>(this Queue<T> queue, out T value)
    {
      if (queue.Count > 0)
      {
        value = queue.Peek();
        return true;
      }

      value = default!;
      return false;
    }

    public static bool TryDequeue<T>(this Queue<T> queue, out T value)
    {
      if (queue.Count > 0)
      {
        value = queue.Dequeue();
        return true;
      }

      value = default!;
      return false;
    }
  }
}