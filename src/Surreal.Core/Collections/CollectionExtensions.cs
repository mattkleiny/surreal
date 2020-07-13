using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Surreal.Mathematics;

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

    public static void ShuffleInPlace<T>(this IList<T> elements, Random random) {
      for (var i = 0; i < elements.Count; i++) {
        // don't select from the entire array on subsequent loops
        var j = random.Next(i, elements.Count);

        elements.Swap(i, j);
      }
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

    public static Queue<T> ToShuffledQueue<T>(this IEnumerable<T> enumerable, Random random) {
      var queue = new Queue<T>();

      foreach (var element in enumerable.Shuffle(random)) {
        queue.Enqueue(element);
      }

      return queue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T SelectRandomly<T>(this IReadOnlyList<T> elements, Random random) {
      if (elements.Count > 0) {
        return elements[random.Next(0, elements.Count)];
      }

      return default!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T SelectRandomly<T>(this IEnumerable<T> elements, Random random) {
      var array = elements.ToArray();

      if (array.Length > 0) {
        return array[random.Next(0, array.Length)];
      }

      return default!;
    }

    public static T? SelectRandomlyWithWeight<T>(this IEnumerable<T> sequence, Random random, Func<T, float> weightProvider)
        where T : class {
      var elements = sequence.ToArray();
      if (elements.Length == 0) return default;

      elements.ShuffleInPlace(random);

      // calculate total cumulative weight of all items
      var totalWeight = 0f;

      for (var i = 0; i < elements.Length; i++) {
        totalWeight += weightProvider(elements[i]);
      }

      // roll the dice and try to find the most likely item
      var targetWeight = random.NextFloat() * totalWeight;

      for (var i = 0; i < elements.Length; i++) {
        var element = elements[i];
        var weight  = weightProvider(element);

        if (targetWeight < weight) {
          return element;
        }

        targetWeight -= weight;
      }

      return elements[^1];
    }

    public static ListSlice<T> SelectAllWithWeight<T>(this IEnumerable<T> sequence, Random random, Func<T, float> weightProvider) {
      var elements = new List<T>(sequence);
      if (elements.Count == 0) return ListSlice<T>.Empty;

      elements.ShuffleInPlace(random);

      var chance = random.NextFloat();

      for (var i = elements.Count - 1; i >= 0; i--) {
        var element = elements[i];
        var weight  = weightProvider(element);

        if (weight < chance) {
          elements.RemoveAt(i);
        }
      }

      return elements;
    }
  }
}