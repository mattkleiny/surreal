using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Surreal.Collections {
  public static class CollectionExtensions {
    public static TValue GetOrCompute<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueProvider) {
      if (!dictionary.ContainsKey(key)) {
        dictionary[key] = valueProvider(key);
      }

      return dictionary[key];
    }

    public static async Task<TValue> GetOrComputeAsync<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, Task<TValue>> asyncValueProvider) {
      if (!dictionary.ContainsKey(key)) {
        dictionary[key] = await asyncValueProvider(key);
      }

      return dictionary[key];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap<T>(this IList<T> array, int fromIndex, int toIndex) {
      var temp = array[fromIndex];

      array[fromIndex] = array[toIndex];
      array[toIndex]   = temp;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T SelectRandomly<T>(this IEnumerable<T> elements, Random random) {
      var array = elements.ToArray();

      if (array.Length > 0) {
        return array[random.Next(0, array.Length)];
      }

      return default!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T SelectRandomly<T>(this IReadOnlyList<T> elements, Random random) {
      if (elements.Count > 0) {
        return elements[random.Next(0, elements.Count)];
      }

      return default!;
    }

    public static T SelectRandomlyWithChance<T>(this IEnumerable<T> sequence, Random random, Func<T, float> chanceProvider) {
      var elements = sequence.ToArray();
      if (elements.Length == 0) return default!;

      // calculate total cumulative weight of all items
      var cumulative = 0f;

      for (var i = 0; i < elements.Length; i++) {
        var element = elements[i];
        cumulative += chanceProvider(element);
      }

      // roll the dice and try to find the most likely item
      var dice = random.NextDouble() * cumulative;

      for (var i = 0; i < elements.Length; i++) {
        var element = elements[i];
        var chance  = chanceProvider(element);

        if (dice < chance) {
          return element;
        }

        dice -= chance;
      }

      return elements[^1];
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> sequence, Random random) {
      var array = sequence.ToArray();

      for (var i = 0; i < array.Length; i++) {
        // don't select from the entire array on subsequent loops
        var j = random.Next(i, array.Length);

        array.Swap(i, j);
      }

      return array;
    }
  }
}