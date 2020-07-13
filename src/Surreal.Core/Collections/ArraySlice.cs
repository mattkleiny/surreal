using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Surreal.Collections {
  public readonly struct ArraySlice<T> : IEnumerable<T> {
    public static ArraySlice<T> Empty => default;

    private readonly T[] array;

    public ArraySlice(T[] array)
        : this(array, 0, array.Length) {
    }

    public ArraySlice(T[] array, int offset, int count) {
      Debug.Assert(offset >= 0, "offset >= 0");
      Debug.Assert(count >= 0, "count >= 0");

      this.array = array;

      Offset = offset;
      Count  = count;
    }

    public int Offset { get; }
    public int Count  { get; }

    public T this[int index] {
      get {
        Debug.Assert(index >= 0, "index >= 0");
        Debug.Assert(index < Count, "index < Count");

        return array[Offset + index];
      }
      set {
        Debug.Assert(index >= 0, "index >= 0");
        Debug.Assert(index < Count, "index < Count");

        array[Offset + index] = value;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Swap(int fromIndex, int toIndex) {
      array.Swap(fromIndex, toIndex);
    }

    public Enumerator             GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public static implicit operator ArraySlice<T>(T[] array) => new ArraySlice<T>(array);

    public struct Enumerator : IEnumerator<T> {
      private readonly ArraySlice<T> slice;
      private          int           index;

      public Enumerator(ArraySlice<T> slice)
          : this() {
        this.slice = slice;
        Reset();
      }

      public T           Current => slice[index];
      object IEnumerator.Current => Current!;

      public bool MoveNext() => ++index < slice.Count;
      public void Reset()    => index = -1;

      public void Dispose() {
      }
    }
  }
}