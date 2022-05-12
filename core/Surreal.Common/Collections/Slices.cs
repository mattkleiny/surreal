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

  public Enumerator GetEnumerator() => new(this);
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

  public static implicit operator Slice<T>(List<T> list) => new(list);
  public static implicit operator ReadOnlySlice<T>(Slice<T> slice) => new(slice.list, slice.Offset, slice.Length);

  /// <summary>An enumerator for a <see cref="Slice{T}"/>.</summary>
  public struct Enumerator : IEnumerator<T>
  {
    private readonly Slice<T> slice;
    private int index;

    public Enumerator(Slice<T> slice)
      : this()
    {
      this.slice = slice;
      Reset();
    }

    public T           Current => slice[index];
    object IEnumerator.Current => Current!;

    public bool MoveNext() => ++index < slice.Length;
    public void Reset() => index = -1;

    public void Dispose()
    {
      // no-op
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

  public Enumerator GetEnumerator() => new(this);
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

  public static implicit operator ReadOnlySlice<T>(T[] array) => new(array);
  public static implicit operator ReadOnlySlice<T>(List<T> list) => new(list);

  /// <summary>An enumerator for <see cref="ReadOnlySlice{T}"/>.</summary>
  public struct Enumerator : IEnumerator<T>
  {
    private readonly ReadOnlySlice<T> slice;
    private int index;

    public Enumerator(ReadOnlySlice<T> slice)
      : this()
    {
      this.slice = slice;
      Reset();
    }

    public T           Current => slice[index];
    object IEnumerator.Current => Current!;

    public bool MoveNext() => ++index < slice.Length;
    public void Reset() => index = -1;

    public void Dispose()
    {
      // no-op
    }
  }
}

/// <summary>Commonly used extensions for <see cref="Slice{T}"/> and <see cref="ReadOnlySlice{T}"/>.</summary>
public static class SliceExtensions
{
  /// <summary>Converts the given list to a slice.</summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Slice<T> AsSlice<T>(this List<T> list) => list;

  /// <summary>Converts the given list to a read-only slice.</summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ReadOnlySlice<T> AsReadOnlySlice<T>(this List<T> list) => list;

  /// <summary>Swaps two elements in-place inside the slice.</summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Swap<T>(this Slice<T> slice, int fromIndex, int toIndex)
  {
    (slice[fromIndex], slice[toIndex]) = (slice[toIndex], slice[fromIndex]);
  }
}
