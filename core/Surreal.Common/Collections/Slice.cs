using System.Runtime.CompilerServices;

namespace Surreal.Collections;

/// <summary>A slice of a <see cref="List{T}"/>.</summary>
public readonly struct Slice<T> : IEnumerable<T>
{
  public static Slice<T> Empty => default;

  private readonly List<T> list;

  public Slice(List<T> list)
    : this(list, 0, list.Count)
  {
  }

  public Slice(List<T> list, int offset, int length)
  {
    Debug.Assert(offset >= 0, "offset >= 0");
    Debug.Assert(length >= 0, "length >= 0");

    this.list = list;

    Offset = offset;
    Length = length;
  }

  public int Offset { get; }
  public int Length { get; }

  public T this[Index index]
  {
    get => list[Offset + index.GetOffset(Length)];
    set => list[Offset + index.GetOffset(Length)] = value;
  }

  public Slice<T> this[Range range]
  {
    get
    {
      var (offset, length) = range.GetOffsetAndLength(Length);

      return new Slice<T>(list, offset, length);
    }
  }

  public Enumerator             GetEnumerator() => new(this);
  IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
  IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

  public static implicit operator Slice<T>(List<T> list) => new(list);

  public static implicit operator ReadOnlySlice<T>(Slice<T> slice) => new(slice.list, slice.Offset, slice.Length);

  public struct Enumerator : IEnumerator<T>
  {
    private readonly Slice<T> slice;
    private          int      index;

    public Enumerator(Slice<T> slice)
      : this()
    {
      this.slice = slice;
      Reset();
    }

    public T           Current => slice[index];
    object IEnumerator.Current => Current!;

    public bool MoveNext() => ++index < slice.Length;
    public void Reset()    => index = -1;

    public void Dispose()
    {
    }
  }
}

/// <summary>A read-only <see cref="Slice{T}"/> variant.</summary>
public readonly struct ReadOnlySlice<T> : IEnumerable<T>
{
  public static ReadOnlySlice<T> Empty => default;

  private readonly IReadOnlyList<T> list;

  public ReadOnlySlice(IReadOnlyList<T> list)
    : this(list, 0, list.Count)
  {
  }

  public ReadOnlySlice(IReadOnlyList<T> list, int offset, int length)
  {
    Debug.Assert(offset >= 0, "offset >= 0");
    Debug.Assert(length >= 0, "length >= 0");

    this.list = list;

    Offset = offset;
    Length = length;
  }

  public int Offset { get; }
  public int Length { get; }

  public T this[Index index] => list[Offset + index.GetOffset(Length)];

  public ReadOnlySlice<T> this[Range range]
  {
    get
    {
      var (offset, length) = range.GetOffsetAndLength(Length);

      return new ReadOnlySlice<T>(list, offset, length);
    }
  }

  public Enumerator             GetEnumerator() => new(this);
  IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();
  IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

  public static implicit operator ReadOnlySlice<T>(T[] array)    => new(array);
  public static implicit operator ReadOnlySlice<T>(List<T> list) => new(list);

  public struct Enumerator : IEnumerator<T>
  {
    private readonly ReadOnlySlice<T> slice;
    private          int              index;

    public Enumerator(ReadOnlySlice<T> slice)
      : this()
    {
      this.slice = slice;
      Reset();
    }

    public T           Current => slice[index];
    object IEnumerator.Current => Current!;

    public bool MoveNext() => ++index < slice.Length;
    public void Reset()    => index = -1;

    public void Dispose()
    {
    }
  }
}

/// <summary>Commonly used extensions for <see cref="Slice{T}"/> and <see cref="ReadOnlySlice{T}"/>.</summary>
public static class SliceExtensions
{
  public static Slice<T>         ToSlice<T>(this List<T> list)                         => ToSlice(list, 0, list.Count);
  public static Slice<T>         ToSlice<T>(this List<T> list, int offset, int length) => new(list, offset, length);
  public static ReadOnlySlice<T> ToReadOnlySlice<T>(this T[] array)                    => ToReadOnlySlice(array, 0, array.Length);

  public static ReadOnlySlice<T> ToReadOnlySlice<T>(this T[] array, int offset, int length)             => new(array, offset, length);
  public static ReadOnlySlice<T> ToReadOnlySlice<T>(this IReadOnlyList<T> list)                         => ToReadOnlySlice(list, 0, list.Count);
  public static ReadOnlySlice<T> ToReadOnlySlice<T>(this IReadOnlyList<T> list, int offset, int length) => new(list, offset, length);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Swap<T>(this Slice<T> slice, int fromIndex, int toIndex)
  {
    (slice[fromIndex], slice[toIndex]) = (slice[toIndex], slice[fromIndex]);
  }
}
