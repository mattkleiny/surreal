namespace Surreal.Collections;

/// <summary>
/// A slice of a <see cref="List{T}" />.
/// </summary>
[DebuggerDisplay("Slice ({Length} items)")]
[CollectionBuilder(typeof(SliceFactory), nameof(SliceFactory.Create))]
public readonly struct Slice<T> : IEnumerable<T>
{
  public static Slice<T> Empty => default;

  private readonly List<T> _list;

  public Slice(List<T> list)
    : this(list, 0, list.Count)
  {
  }

  public Slice(List<T> list, int offset, int length)
  {
    Debug.Assert(offset >= 0, "offset >= 0");
    Debug.Assert(length >= 0, "length >= 0");

    _list = list;

    Offset = offset;
    Length = length;
  }

  public int Offset { get; }
  public int Length { get; }

  public T this[Index index]
  {
    get => _list[Offset + index.GetOffset(Length)];
    set => _list[Offset + index.GetOffset(Length)] = value;
  }

  public Slice<T> this[Range range]
  {
    get
    {
      var (offset, length) = range.GetOffsetAndLength(Length);

      return new Slice<T>(_list, offset, length);
    }
  }

  public Enumerator GetEnumerator()
  {
    return new Enumerator(this);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return GetEnumerator();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator Slice<T>(List<T> list) => new(list);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator ReadOnlySlice<T>(Slice<T> slice) => new(slice._list, slice.Offset, slice.Length);

  /// <summary>
  /// An enumerator for a <see cref="Slice{T}" />.
  /// </summary>
  public struct Enumerator : IEnumerator<T>
  {
    private readonly Slice<T> _slice;
    private int _index;

    public Enumerator(Slice<T> slice)
      : this()
    {
      _slice = slice;
      Reset();
    }

    public T Current => _slice[_index];
    object IEnumerator.Current => Current!;

    public bool MoveNext()
    {
      return ++_index < _slice.Length;
    }

    public void Reset()
    {
      _index = -1;
    }

    public void Dispose()
    {
      // no-op
    }
  }
}

/// <summary>
/// A read-only <see cref="Slice{T}" /> variant.
/// </summary>
[DebuggerDisplay("ReadOnlySlice ({Length} items)")]
[CollectionBuilder(typeof(SliceFactory), nameof(SliceFactory.CreateReadOnly))]
public readonly struct ReadOnlySlice<T> : IEnumerable<T>
{
  public static ReadOnlySlice<T> Empty => default;

  private readonly IReadOnlyList<T> _list;

  public ReadOnlySlice(IReadOnlyList<T> list)
    : this(list, 0, list.Count)
  {
  }

  public ReadOnlySlice(IReadOnlyList<T> list, int offset, int length)
  {
    Debug.Assert(offset >= 0, "offset >= 0");
    Debug.Assert(length >= 0, "length >= 0");

    _list = list;

    Offset = offset;
    Length = length;
  }

  public int Offset { get; }
  public int Length { get; }

  public T this[Index index] => _list[Offset + index.GetOffset(Length)];

  public ReadOnlySlice<T> this[Range range]
  {
    get
    {
      var (offset, length) = range.GetOffsetAndLength(Length);

      return new ReadOnlySlice<T>(_list, offset, length);
    }
  }

  public Enumerator GetEnumerator()
  {
    return new Enumerator(this);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return GetEnumerator();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator ReadOnlySlice<T>(T[] array) => new(array);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator ReadOnlySlice<T>(List<T> list) => new(list);

  /// <summary>
  /// An enumerator for <see cref="ReadOnlySlice{T}" />.
  /// </summary>
  public struct Enumerator : IEnumerator<T>
  {
    private readonly ReadOnlySlice<T> _slice;
    private int _index;

    public Enumerator(ReadOnlySlice<T> slice)
      : this()
    {
      _slice = slice;
      Reset();
    }

    public T Current => _slice[_index];
    object IEnumerator.Current => Current!;

    public bool MoveNext()
    {
      return ++_index < _slice.Length;
    }

    public void Reset()
    {
      _index = -1;
    }

    public void Dispose()
    {
      // no-op
    }
  }
}

/// <summary>
/// Commonly used extensions for <see cref="Slice{T}" /> and <see cref="ReadOnlySlice{T}" />.
/// </summary>
public static class SliceExtensions
{
  /// <summary>
  /// Converts the given list to a slice.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Slice<T> AsSlice<T>(this List<T> list)
    => list;

  /// <summary>
  /// Converts the given list to a slice with the given offset and length.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Slice<T> AsSlice<T>(this List<T> list, int offset, int length)
    => new(list, offset, length);

  /// <summary>
  /// Converts the given list to a read-only slice.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ReadOnlySlice<T> AsReadOnlySlice<T>(this List<T> list)
    => list;

  /// <summary>
  /// Converts the given list to a read-only slice with the given offset and length.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ReadOnlySlice<T> AsReadOnlySlice<T>(this List<T> list, int offset, int length)
    => new(list, offset, length);

  /// <summary>
  /// Converts the given enumerable to a slice.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Slice<T> ToSlice<T>(this IEnumerable<T> enumerable)
    => enumerable.ToList().AsSlice();

  /// <summary>
  /// Converts the given enumerable to a slice with the given offset and length.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Slice<T> ToSlice<T>(this IEnumerable<T> enumerable, int offset, int length)
    => enumerable.ToList().AsSlice(offset, length);

  /// <summary>
  /// Converts the given enumerable to a read-only slice.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ReadOnlySlice<T> ToReadOnlySlice<T>(this IEnumerable<T> enumerable)
    => enumerable.ToList().AsReadOnlySlice();

  /// <summary>
  /// Converts the given enumerable to a read-only slice with the given offset and length.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ReadOnlySlice<T> ToReadOnlySlice<T>(this IEnumerable<T> enumerable, int offset, int length)
    => enumerable.ToList().AsReadOnlySlice(offset, length);

  /// <summary>
  /// Swaps two elements in-place inside the slice.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Swap<T>(this Slice<T> slice, int fromIndex, int toIndex)
    => (slice[fromIndex], slice[toIndex]) = (slice[toIndex], slice[fromIndex]);
}

/// <summary>
/// Collection builder for <see cref="Slice{T}"/>s.
/// </summary>
public static class SliceFactory
{
  /// <summary>
  /// Creates a new <see cref="Slice{T}"/> from the given values.
  /// </summary>
  public static Slice<T> Create<T>(ReadOnlySpan<T> values)
  {
    if (values.Length == 0)
    {
      return Slice<T>.Empty;
    }

    return new Slice<T>(new List<T>(values.ToArray()));
  }

  /// <summary>
  /// Creates a new <see cref="ReadOnlySlice{T}"/> from the given values.
  /// </summary>
  public static ReadOnlySlice<T> CreateReadOnly<T>(ReadOnlySpan<T> values)
  {
    if (values.Length == 0)
    {
      return ReadOnlySlice<T>.Empty;
    }

    return new ReadOnlySlice<T>(values.ToArray());
  }
}
