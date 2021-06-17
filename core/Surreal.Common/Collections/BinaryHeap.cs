using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Surreal.Collections {
  [DebuggerDisplay("BinaryHeap {Count} items")]
  public sealed class BinaryHeap<T, TCost>
      where T : notnull
      where TCost : notnull {
    private const int GrowthFactor = 2;

    private readonly IComparer<TCost> comparer;
    private          Item[]           items;

    public BinaryHeap(IComparer<TCost> comparer, int capacity = 32) {
      if (capacity < 1) {
        throw new ArgumentException("Capacity must be greater than zero", nameof(capacity));
      }

      this.comparer = comparer;

      items = new Item[capacity];
    }

    public int Count { get; private set; }

    public int Capacity {
      get => items.Length;
      set => SetCapacity(value);
    }

    public void Push(T item, TCost cost) {
      if (Count == Capacity) {
        Capacity *= GrowthFactor;
      }

      items[Count].Set(item, cost);

      BubbleUp();

      Count++;
    }

    public T Peek() {
      if (Count == 0) {
        throw new InvalidOperationException("Cannot peek at first item, heap is empty.");
      }

      return items[0].Data;
    }

    public T Pop() {
      if (Count == 0) {
        throw new InvalidOperationException("Cannot remove item, heap is empty.");
      }

      var data = items[0].Data;

      Count -= 1;

      items[0] = items[Count];

      items[Count].Data = default!;
      items[Count].Cost = default!;

      BubbleDown();

      return data;
    }

    public bool TryPop(out T value) {
      if (Count == 0) {
        value = default!;
        return false;
      }

      value = Pop();
      return true;
    }

    public void Clear() {
      Count = 0;

      Array.Clear(items, 0, Capacity);
    }

    private void SetCapacity(int newCapacity) {
      newCapacity = Math.Max(newCapacity, Count);

      if (Capacity != newCapacity) {
        Array.Resize(ref items, newCapacity);
      }
    }

    private void BubbleUp() {
      var index  = Count;
      var item   = items[index];
      var parent = (index - 1) / 2;

      while (parent > -1 && comparer.Compare(item.Cost, items[parent].Cost) < 0) {
        items[index] = items[parent]; // Swap nodes

        index  = parent;
        parent = (index - 1) / 2;

        if (parent == index) {
          break; // HACK: somehow we end up in this situation
        }
      }

      items[index] = item;
    }

    private void BubbleDown() {
      var parent = 0;
      var item   = items[parent];

      while (true) {
        var ch1 = parent * 2 + 1;
        if (ch1 >= Count) break;

        var ch2 = parent * 2 + 2;
        int index;

        if (ch2 >= Count) {
          index = ch1;
        }
        else {
          index = comparer.Compare(items[ch1].Cost, items[ch2].Cost) < 0 ? ch1 : ch2;
        }

        if (comparer.Compare(item.Cost, items[index].Cost) < 0) {
          break;
        }

        items[parent] = items[index]; // Swap nodes
        parent        = index;
      }

      items[parent] = item;
    }

    private struct Item {
      public T     Data;
      public TCost Cost;

      public void Set(T data, TCost cost) {
        Data = data;
        Cost = cost;
      }
    }
  }
}