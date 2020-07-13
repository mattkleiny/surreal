using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Surreal.Collections {
  public readonly struct ListSlice<T> : IEnumerable<T> {
    public static ListSlice<T> Empty => default;

    private readonly List<T> list;

    public ListSlice(List<T> list)
        : this(list, 0, list.Count) {
    }

    public ListSlice(List<T> list, int offset, int count) {
      Debug.Assert(offset >= 0, "offset >= 0");
      Debug.Assert(count >= 0, "count >= 0");

      this.list = list;

      Offset = offset;
      Count  = count;
    }

    public int Offset { get; }
    public int Count  { get; }

    public T this[int index] {
      get {
        Debug.Assert(index >= 0, "index >= 0");
        Debug.Assert(index < Count, "index < Count");

        return list[Offset + index];
      }
      set {
        Debug.Assert(index >= 0, "index >= 0");
        Debug.Assert(index < Count, "index < Count");

        list[Offset + index] = value;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Swap(int fromIndex, int toIndex) {
      list.Swap(fromIndex, toIndex);
    }

    public Enumerator             GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public static implicit operator ListSlice<T>(List<T> list) => new ListSlice<T>(list);

    public struct Enumerator : IEnumerator<T> {
      private readonly ListSlice<T> slice;
      private          int          index;

      public Enumerator(ListSlice<T> slice)
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