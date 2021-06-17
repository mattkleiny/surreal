using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Surreal.Collections {
  public readonly struct Slice<T> : IReadOnlyList<T> {
    public static Slice<T> Empty => default;

    private readonly IList<T> list;

    public Slice(IList<T> list)
        : this(list, 0, list.Count) {
    }

    public Slice(IList<T> list, int offset, int length) {
      Debug.Assert(offset >= 0, "offset >= 0");
      Debug.Assert(length >= 0, "length >= 0");

      this.list = list;

      Offset = offset;
      Length = length;
    }

    public int Offset { get; }
    public int Length { get; }

    public T this[int index] {
      get => list[Offset + index];
      set => list[Offset + index] = value;
    }

    public Slice<T> Take(int offset, int length) {
      return new(list, Offset + offset, length);
    }

    int IReadOnlyCollection<T>.Count => Length;

    public Enumerator             GetEnumerator() => new(this);
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public static implicit operator Slice<T>(T[] array)    => new(array);
    public static implicit operator Slice<T>(List<T> list) => new(list);

    public static implicit operator ReadOnlySlice<T>(Slice<T> slice) => new((IReadOnlyList<T>) slice.list, slice.Offset, slice.Length);

    public struct Enumerator : IEnumerator<T> {
      private readonly Slice<T> slice;
      private          int      index;

      public Enumerator(Slice<T> slice)
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

  public readonly struct ReadOnlySlice<T> : IReadOnlyList<T> {
    public static ReadOnlySlice<T> Empty => default;

    private readonly IReadOnlyList<T> list;

    public ReadOnlySlice(IReadOnlyList<T> list)
        : this(list, 0, list.Count) {
    }

    public ReadOnlySlice(IReadOnlyList<T> list, int offset, int length) {
      Debug.Assert(offset >= 0, "offset >= 0");
      Debug.Assert(length >= 0, "length >= 0");

      this.list = list;

      Offset = offset;
      Length = length;
    }

    public int Offset { get; }
    public int Length { get; }

    public T this[int index] => list[Offset + index];

    public ReadOnlySlice<T> Slice(int offset, int length) {
      return new(list, Offset + offset, length);
    }

    int IReadOnlyCollection<T>.Count => Length;

    public Enumerator             GetEnumerator() => new(this);
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public static implicit operator ReadOnlySlice<T>(T[] array)    => new(array);
    public static implicit operator ReadOnlySlice<T>(List<T> list) => new(list);

    public struct Enumerator : IEnumerator<T> {
      private readonly ReadOnlySlice<T> slice;
      private          int              index;

      public Enumerator(ReadOnlySlice<T> slice)
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

  public static class SliceExtensions {
    public static Slice<T> ToSlice<T>(this T[] array)                             => ToSlice(array, 0, array.Length);
    public static Slice<T> ToSlice<T>(this T[] array, int offset, int length)     => new(array, offset, length);
    public static Slice<T> ToSlice<T>(this IList<T> list)                         => ToSlice(list, 0, list.Count);
    public static Slice<T> ToSlice<T>(this IList<T> list, int offset, int length) => new(list, offset, length);

    public static ReadOnlySlice<T> ToReadOnlySlice<T>(this T[] array)                                     => ToReadOnlySlice(array, 0, array.Length);
    public static ReadOnlySlice<T> ToReadOnlySlice<T>(this T[] array, int offset, int length)             => new(array, offset, length);
    public static ReadOnlySlice<T> ToReadOnlySlice<T>(this IReadOnlyList<T> list)                         => ToReadOnlySlice(list, 0, list.Count);
    public static ReadOnlySlice<T> ToReadOnlySlice<T>(this IReadOnlyList<T> list, int offset, int length) => new(list, offset, length);

    public static void Swap<T>(this Slice<T> slice, int fromIndex, int toIndex) {
      var temp = slice[fromIndex];

      slice[fromIndex] = slice[toIndex];
      slice[toIndex]   = temp;
    }

    public static T? SelectRandomly<T>(this ReadOnlySlice<T?> elements, Random random) {
      if (elements.Length > 0) {
        return elements[random.Next(0, elements.Length)];
      }

      return default;
    }

    public static void ShuffleInPlace<T>(this Slice<T?> elements, Random random) {
      for (var i = 0; i < elements.Length; i++) {
        // don't select from the entire array on subsequent loops
        var j = random.Next(i, elements.Length);

        elements.Swap(i, j);
      }
    }

    public static Slice<T?> Reverse<T>(this Slice<T?> slice) {
      var start = 0;
      var end   = slice.Length - 1;

      while (start < end) {
        slice.Swap(start, end);

        start++;
        end--;
      }

      return slice;
    }
  }
}