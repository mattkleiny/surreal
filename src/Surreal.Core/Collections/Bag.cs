using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Surreal.Collections {
  [DebuggerDisplay("Bag with {Count} elements")]
  public sealed class Bag<T> : IReadOnlyList<T> {
    private T[] elements;

    public Bag(int capacity = 16) {
      Debug.Assert(capacity >= 0, "capacity >= 0");

      elements = new T[capacity];

      Count = 0;
    }

    public Span<T> Span  => new Span<T>(elements, 0, Count);
    public int     Count { get; private set; }

    public T this[int index] {
      get => elements[index];
      set {
        if (index >= elements.Length) Grow(index * 2);
        if (index >= Count) Count = index + 1;

        elements[index] = value;
      }
    }

    public ref T Get(int index) => ref elements[index];

    public void Add(T element) {
      if (Count == elements.Length) {
        Grow();
      }

      elements[Count] = element;
      Count++;
    }

    public void Remove(T element) {
      for (var index = elements.Length - 1; index >= 0; index--) {
        if (Equals(elements[index], element)) {
          --Count;

          elements[index] = elements[Count];
          elements[Count] = default!;

          break;
        }
      }
    }

    public T Remove(int index) {
      var result = elements[index];

      --Count;

      elements[index] = elements[Count];
      elements[Count] = default!;

      return result;
    }

    public void Sort(Comparison<T> comparison) {
      Array.Sort(elements, Comparer<T>.Create(comparison));
    }

    public void Clear() {
      for (var index = Count - 1; index >= 0; index--) {
        elements[index] = default!;
      }

      Count = 0;
    }

    private void Grow() {
      Grow((int) (elements.Length * 1.5) + 1);
    }

    private void Grow(int newCapacity) {
      var oldElements = elements;
      elements = new T[newCapacity];

      Array.Copy(oldElements, 0, elements, 0, oldElements.Length);
    }

    public Enumerator             GetEnumerator() => new Enumerator(this);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<T> {
      private readonly Bag<T> bag;
      private          int    index;

      public Enumerator(Bag<T> bag)
          : this() {
        this.bag = bag;
        Reset();
      }

      public T           Current    => bag[index];
      object IEnumerator.Current    => Current!;
      public bool        MoveNext() => ++index < bag.Count;
      public void        Reset()    => index = -1;

      public void Dispose() {
      }
    }
  }
}