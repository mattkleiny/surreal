using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Surreal.Collections {
  public sealed class Array<T> : IList<T>, IReadOnlyList<T> {
    private T[] elements;

    public Array(int initialSize = 0) {
      elements = new T[initialSize];
    }

    public Array(params T[] elements) {
      this.elements = elements;

      Count = elements.Length;
    }

    public T[] Elements => elements;
    public int Count    { get; private set; }

    public int Capacity {
      get => elements.Length;
      set {
        var length = Math.Max(0, value);

        if (elements.Length != length) {
          Array.Resize(ref elements, length);
        }
      }
    }

    public ref T this[int index] => ref elements[index];

    T IReadOnlyList<T>.this[int index] => this[index];

    T IList<T>.this[int index] {
      get => this[index];
      set => this[index] = value;
    }

    bool ICollection<T>.IsReadOnly => false;

    public void Add(T item) {
      if (Count >= Capacity) {
        Grow();
      }

      elements[Count++] = item;
    }

    public bool Contains(T item) {
      return IndexOf(item) != -1;
    }

    public int IndexOf(T item) {
      for (var i = 0; i < elements.Length; i++) {
        if (Equals(elements[i], item)) {
          return i;
        }
      }

      return -1;
    }

    public void CopyTo(T[] array, int arrayIndex) {
      Array.Copy(elements, 0, array, arrayIndex, Count);
    }

    public void Insert(int index, T item) {
      throw new NotSupportedException();
    }

    public void RemoveAt(int index) {
      Debug.Assert(index >= 0, "index >= 0");

      // we've removed this element
      elements[index] = default!;

      // bubble the rest up into the empty spot
      for (var i = index; i < Count - 1; i++) {
        elements[i] = elements[i + 1];
      }

      // the last element is no longer valid
      Count--;
    }

    public bool Remove(T item) {
      var index = IndexOf(item);

      if (index != -1) {
        RemoveAt(index);
        return true;
      }

      return false;
    }

    public void Clear() {
      for (var i = 0; i < Count; i++) {
        elements[i] = default!; // help the GC
      }

      Count = 0;
    }

    private void Grow() {
      Capacity = (int) (Count * 1.5) + 1;
    }

    public Slice<T> ToSlice() => new(elements, 0, Count);
    public Span<T>  ToSpan()  => new(elements, 0, Count);

    public Enumerator             GetEnumerator() => new(this);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<T> {
      private readonly Array<T> array;
      private          int      index;

      public Enumerator(Array<T> array)
          : this() {
        this.array = array;

        Reset();
      }

      public ref T       Current => ref array.elements[index];
      T IEnumerator<T>.  Current => array.elements[index];
      object IEnumerator.Current => Current!;

      public bool MoveNext() => ++index < array.Count;
      public void Reset()    => index = -1;

      public void Dispose() {
      }
    }
  }
}