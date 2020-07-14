using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Surreal.Collections {
  public readonly struct ArraySlice<T> : IEnumerable<T> {
    public static ArraySlice<T> Empty => default;

    private readonly T[] array;

    public ArraySlice(T[] array)
      : this(array, 0, array.Length) {
    }

    public ArraySlice(T[] array, int offset, int length) {
      Debug.Assert(offset >= 0, "offset >= 0");
      Debug.Assert(length >= 0, "count >= 0");

      this.array = array;

      Offset = offset;
      Length = length;
    }

    public int Offset { get; }
    public int Length { get; }

    public T this[Index index] {
      get => array[Offset + index.GetOffset(Length)];
      set => array[Offset + index.GetOffset(Length)] = value;
    }

    public ArraySlice<T> this[Range range] {
      get {
        var (offset, length) = range.GetOffsetAndLength(Length);

        return new ArraySlice<T>(array, offset, length);
      }
    }

    public Enumerator GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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

      public bool MoveNext() => ++index < slice.Length;
      public void Reset() => index = -1;

      public void Dispose() {
      }
    }
  }
}