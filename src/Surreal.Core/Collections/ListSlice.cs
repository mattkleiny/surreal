using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Surreal.Collections {
  public readonly struct ListSlice<T> : IEnumerable<T> {
    public static ListSlice<T> Empty => default;

    private readonly IReadOnlyList<T> list;

    public ListSlice(IReadOnlyList<T> list)
        : this(list, 0, list.Count) {
    }

    public ListSlice(IReadOnlyList<T> list, int offset, int length) {
      Debug.Assert(offset >= 0, "offset >= 0");
      Debug.Assert(length >= 0, "count >= 0");

      this.list = list;

      Offset = offset;
      Length = length;
    }

    public int Offset { get; }
    public int Length { get; }

    public T this[Index index] => list[Offset + index.GetOffset(Length)];

    public ListSlice<T> this[Range range] {
      get {
        var (offset, length) = range.GetOffsetAndLength(Length);

        return new ListSlice<T>(list, offset, length);
      }
    }

    public Enumerator             GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public static implicit operator ListSlice<T>(T[] list)     => new ListSlice<T>(list);
    public static implicit operator ListSlice<T>(List<T> list) => new ListSlice<T>(list);
    public static implicit operator ListSlice<T>(Bag<T> list)  => new ListSlice<T>(list);

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

      public bool MoveNext() => ++index < slice.Length;
      public void Reset()    => index = -1;

      public void Dispose() {
      }
    }
  }
}