using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Surreal.Collections {
  public readonly struct ListSpan<T> : IEnumerable<T> {
    public static ListSpan<T> Empty => default;

    private readonly IReadOnlyList<T> list;

    public ListSpan(IReadOnlyList<T> list)
        : this(list, 0, list.Count) {
    }

    public ListSpan(IReadOnlyList<T> list, Range range) {
      var (offset, length) = range.GetOffsetAndLength(list.Count);

      this.list = list;

      Offset = offset;
      Length = length;
    }

    public ListSpan(IReadOnlyList<T> list, int offset, int length) {
      Debug.Assert(offset >= 0, "offset >= 0");
      Debug.Assert(length >= 0, "count >= 0");

      this.list = list;

      Offset = offset;
      Length = length;
    }

    public int Offset { get; }
    public int Length { get; }

    public T this[Index index] => list[Offset + index.GetOffset(Length)];

    public ListSpan<T> this[Range range] {
      get {
        var (offset, length) = range.GetOffsetAndLength(Length);

        return new ListSpan<T>(list, offset, length);
      }
    }

    public Enumerator             GetEnumerator() => new(this);
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public static implicit operator ListSpan<T>(T[] list)     => new(list);
    public static implicit operator ListSpan<T>(List<T> list) => new(list);
    public static implicit operator ListSpan<T>(Bag<T> list)  => new(list);

    public struct Enumerator : IEnumerator<T> {
      private readonly ListSpan<T> span;
      private          int         index;

      public Enumerator(ListSpan<T> span)
          : this() {
        this.span = span;
        Reset();
      }

      public T           Current => span[index];
      object IEnumerator.Current => Current!;

      public bool MoveNext() => ++index < span.Length;
      public void Reset()    => index = -1;

      public void Dispose() {
      }
    }
  }
}