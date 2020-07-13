using System.Collections.Generic;

namespace Surreal.Collections {
  public sealed class PriorityQueue<T> {
    private readonly List<(T Item, float Priority)> elements = new List<(T, float)>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority) {
      elements.Add((item, priority));
    }

    public T Dequeue() {
      if (elements.Count == 0) return default!;

      var bestIndex = 0;
      for (var i = 0; i < elements.Count; i++) {
        if (elements[i].Priority < elements[bestIndex].Priority) {
          bestIndex = i;
        }
      }

      var bestItem = elements[bestIndex].Item;
      elements.RemoveAt(bestIndex);

      return bestItem;
    }

    public void Clear() {
      elements.Clear();
    }
  }
}